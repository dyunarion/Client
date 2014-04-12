using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HTTP;
using System.Linq;

public class SocketIOClient : MonoBehaviour
{
    public delegate void EventHandler(SocketIOClient socket, IMessage msg);

    public string url;
    public bool reconnect;
    public string sid;
    public float heartbeatTimeout;
    public float closingTimeout;
    public string[] transports;
    Dictionary<string, EventHandler> handlers = new Dictionary<string, EventHandler>();
    WebSocket socket;
    int msgUid = 0;

    public bool Ready
    {
        get
        {
            return socket != null;
        }
    }

    void OnApplicationQuit()
    {
        if (socket != null)
            socket.Close(WebSocket.CloseEventCode.CloseEventCodeGoingAway, "Bye.");
    }

    public void On(string eventName, EventHandler fn)
    {
        handlers[eventName] = fn;
    }

    public int Send(IMessage msg)
    {
        msg.id = msgUid++;
        if (socket == null)
        {
            Debug.LogError("Socket.IO is not initialised yet!");
            return -1;
        }
        else
        {
            socket.Send(msg.ToString());
            return msg.id.Value;
        }
    }

    public int Send(object payload)
    {
        var m = new IMessage();
        m.type = IMessage.FrameType.JSONMESSAGE;
        m.data = HTTP.JsonSerializer.Encode(payload);
        return Send(m);
    }

    public int Emit(string eventName, params object[] args)
    {
        var m = new IMessage();
        m.type = IMessage.FrameType.EVENT;
        var payload = new Hashtable();
        payload["name"] = eventName;
        payload["args"] = args;
        m.data = HTTP.JsonSerializer.Encode(payload);
        return Send(m);
    }

    void Start()
    {
        Application.runInBackground = true;
        if (!url.EndsWith("/"))
        {
            url = url + "/";
        }
        Dispatch("connecting", null);
        StartCoroutine(EstablishConnection(
        delegate()
        {
            Dispatch("connect", null);
        },
        delegate
        {
            Dispatch("connect_failed", null);
        }));
    }

    void Reconnect()
    {
        Dispatch("reconnecting", null);
        StartCoroutine(EstablishConnection(
            delegate()
            {
                Dispatch("reconnect", null);
            },
        delegate
        {
            Dispatch("reconnect_failed", null);
        }));
    }

    IEnumerator EstablishConnection(System.Action success, System.Action failed)
    {
        var req = new HTTP.Request("POST", url + "socket.io/1/");
        yield return req.Send();
        if (req.exception == null)
        {
            if (req.response.status == 200)
            {
                var parts = (from i in req.response.Text.Split(':') select i.Trim()).ToArray();
                sid = parts[0];
                float.TryParse(parts[1], out heartbeatTimeout);
                float.TryParse(parts[2], out closingTimeout);
                transports = (from i in parts[3].Split(',') select i.Trim().ToLower()).ToArray();
            }
            if (transports.Contains("websocket"))
            {
                socket = new WebSocket();
                StartCoroutine(socket.Dispatcher());
                socket.Connect(url + "socket.io/1/websocket/" + sid);
                socket.OnTextMessageRecv += HandleSocketOnTextMessageRecv;
                socket.OnDisconnect += delegate()
                {
                    Dispatch("disconnect", null);
                };
                success();
            }
            else
            {
                failed();
                Debug.LogError("Websocket is not supported with this server.");
            }
        }
        else
        {
            failed();
        }
    }

    void Dispatch(string eventName, IMessage msg)
    {
        EventHandler fn = null;
        if (handlers.TryGetValue(eventName, out fn))
        {
            fn(this, msg);
        }
    }

    void HandleSocketOnTextMessageRecv(string message)
    {

        var msg = IMessage.FromString(message);
        msg.socket = this;

        switch (msg.type)
        {
            case IMessage.FrameType.DISCONNECT:
                StopCoroutine("Hearbeat");
                Dispatch("disconnect", null);
                if (reconnect)
                {
                    Reconnect();
                }
                break;
            case IMessage.FrameType.CONNECT:
                if (msg.endPoint == null)
                    StartCoroutine("Heartbeat");
                break;
            case IMessage.FrameType.HEARTBEAT:
                //
                break;
            case IMessage.FrameType.MESSAGE:
                Dispatch("message", msg);
                break;
            case IMessage.FrameType.JSONMESSAGE:
                Dispatch("json_message", msg);
                break;
            case IMessage.FrameType.EVENT:
                Dispatch(msg.eventName, msg);
                break;
            case IMessage.FrameType.ACK:
                //
                break;
            case IMessage.FrameType.ERROR:
                Dispatch("error", msg);
                break;
            case IMessage.FrameType.NOOP:
                break;
            default:
                break;
        }
    }

    IEnumerator Heartbeat()
    {
        var beat = new IMessage();
        beat.type = IMessage.FrameType.HEARTBEAT;
        var delay = new WaitForSeconds(heartbeatTimeout * 0.8f);
        while (socket.connected)
        {
            socket.Send(beat.ToString());
            yield return delay;
        }
    }
}