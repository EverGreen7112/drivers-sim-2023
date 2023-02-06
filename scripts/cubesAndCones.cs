using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubesAndCones
{
    private static UdpSocketManager _udpSocketManager;
    private static bool _isListenPortLogged = false;
    private static Vector3[] _cubes = new Vector3[]{new Vector3(0,0,0)};
    private static Vector3[] _cones = new Vector3[]{new Vector3(0,0,0)};

    // public static IEnumerator StartReading() {
    //     _udpSocketManager = new UdpSocketManager("127.0.0.1", PORT, PORT);
    //     _udpSocketManager.initSocket();
    //     return receiveStream();
    // }
    public cubesAndCones(string serverIp, int serverPort, int clientPort) {
        
        _udpSocketManager = new UdpSocketManager(serverIp, serverPort, clientPort);
        _udpSocketManager.initSocket();
        
    }

    public IEnumerator receiveStream() {
        while (!_udpSocketManager.isInitializationCompleted()) {
            yield return null;
        }
        

        if (!_isListenPortLogged) {
            Debug.Log("UdpSocketManager, listen port: " + _udpSocketManager.getListenPort());
            _isListenPortLogged = true;
        }

        // BitwiseMemoryOutputStream outStream = new BitwiseMemoryOutputStream();
        // outStream.writeBool(true);
        // outStream.writeByte(0xfa);
        // outStream.writeDouble(1.2);
        // outStream.writeFloat(81.12f);
        // outStream.writeInt(7, 3);
        // outStream.writeLong(8, 4);
        // outStream.writeSigned(-7, 3);
        // outStream.writeSignedLong(-8, 4);
        // outStream.writeString("Hello World!");
        // Debug.Log("UdpSocketManager, stream have sent!");

        // _udpSocketManager.send(outStream.getBuffer());
        while (true) {
            try {
                IList<byte[]> recPackets = _udpSocketManager.receive();

                while (recPackets.Count < 1) {
                    yield return null;
                    recPackets = _udpSocketManager.receive();
                }

                byte[] echoPacket = recPackets[0];

                BitwiseMemoryInputStream inStream = new BitwiseMemoryInputStream(echoPacket);
                int conesAmmount = inStream.readInt();
                int cubesAmmount = inStream.readInt();
                Vector3[] cones = new Vector3[conesAmmount];
                Vector3[] cubes = new Vector3[cubesAmmount];
                // cones
                for (int i = 0; i < conesAmmount; i++){
                    cones[i] = new Vector3(inStream.readFloat(),
                    inStream.readFloat(), 
                    inStream.readFloat());
                }
                // cubes
                for (int i = 0; i < cubesAmmount; i++){
                    cubes[i] = new Vector3(inStream.readFloat(),
                    inStream.readFloat(), 
                    inStream.readFloat());
                }
                _cones = cones;
                _cubes = cubes;

                Debug.Log("finished read");
            }
            finally {
                // Debug.Log("failed");
            }
        }
    }


    public static Vector3[] Cones {get{return _cones;} }
    public static Vector3[] Cubes {get{return _cubes;} }

    public static void StopReading() {
        if(_udpSocketManager != null) {
            _udpSocketManager.closeSocketThreads();
        }
    }
    
}
