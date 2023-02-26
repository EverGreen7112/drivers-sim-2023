using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class Read_Input : MonoBehaviour
{
    private UdpSocketManager _udpSocketManager;
    private bool _isListenPortLogged = false;
    public bool useAngle = true;
    public bool useLocation = true;
    public Vector3 location_weights = new Vector3(1f, 1f, 1f);
    public Vector3 angle_weights = new Vector3(1f, 1f, 1f);
    public int PORT = 5804;
    public Vector3 target_location;
    public Vector3 target_angle;
    private UdpClient udpClient;
    // private CharacterController controller; 
    // private IPEndPoint groupEP;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {

        _udpSocketManager = new UdpSocketManager("0.0.0.0", PORT, PORT);
        StartCoroutine(_udpSocketManager.initSocket());
        StartCoroutine(receiveStream());
        // controller = gameObject.GetComponent<CharacterController>();
        

        // udpClient = new UdpClient(PORT);
        // groupEP = new IPEndPoint(IPAddress.None, PORT);
        // udpClient.Client.Bind(groupEP);
        //udpClient.JoinMulticastGroup(IPAddress.Any);
        target_location = transform.position;
        target_angle = transform.rotation.eulerAngles; 

        // offset = transform.position;
    }

    IEnumerator receiveStream() {
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
                    // Debug.Log(recPackets);
                }
                Debug.Log("read input");
                byte[] echoPacket = recPackets[0];
                BitwiseMemoryInputStream inStream = new BitwiseMemoryInputStream(echoPacket);
                if (useLocation){
                    try{
                            var temp = new Vector3(inStream.readFloat(),
                            inStream.readFloat(), 
                            inStream.readFloat());
                            target_location = Vector3.Scale(temp, location_weights);
                    }
                    catch {}
                }
                if (useAngle){
                    try{
                            var temp = new Vector3(inStream.readFloat(),
                            inStream.readFloat(),
                            inStream.readFloat()
                            // 
                            );
                            target_angle = Vector3.Scale(temp, angle_weights);
                    }
                    catch {}
                }
                // Debug.Log("finished read");
            }
            finally {
                    Debug.Log("failed");
            }
        }
        // Debug.Assert(inStream.readBool() == true);
        // Debug.Assert(inStream.readByte() == 0xfa);
        // Debug.Assert(inStream.readDouble() == 1.2);
        // Debug.Assert(inStream.readFloat() == 81.12f);
        // Debug.Assert(inStream.readInt(3) == 7);
        // Debug.Assert(inStream.readLong(4) == 8);
        // Debug.Assert(inStream.readSignedInt(3) == -7);
        // Debug.Assert(inStream.readSignedLong(4) == -8);
        // Debug.Assert(inStream.readString() == "Hello World!");
        // Debug.Log("UdpSocketManager, stream have received!");
    }


    // async void ReadTargets(){
    //     await Task.Run(() => {
    //         byte[] bytes;
    //         try{
    //             bytes = udpClient.Receive(ref groupEP);
    //             //Debug.Log($"x: {BitConverter.ToSingle( bytes, 0 )} y: {BitConverter.ToSingle( bytes, 4 )} z: {BitConverter.ToSingle( bytes, 8)}");
    //             if (useLocation){
    //                 var temp = new Vector3(BitConverter.ToSingle(bytes, 8),
    //                 BitConverter.ToSingle( bytes, 4 ), 
    //                 BitConverter.ToSingle( bytes, 0 ));
    //                 target_location = Vector3.Scale(temp, location_weights);
    //             }
    //             if (useAngle){
    //                 var temp = new Vector3(BitConverter.ToSingle( bytes, 20 ),
    //                 BitConverter.ToSingle( bytes, 16 ),
    //                 BitConverter.ToSingle( bytes, 12)
    //                 // 
    //                 );
    //                 target_angle = Vector3.Scale(temp, angle_weights);
    //             }
                
                
    //         } 
    //         catch{
    //             Debug.Log("failed");
    //         } 
    //     });
        
    // }

    // Update is called once per frame
    void Update()
    {
        // this.ReadTargets();
        if (useLocation)
        {
            transform.position = (target_location + offset);
        }
        if (useAngle) 
        {
            // transform.Rotate(target_angle);
            // transform.localEulerAngles = target_angle;
            // transform.eulerAngles = target_angle;
            transform.Rotate(target_angle - transform.eulerAngles, Space.Self);
        }
        
    }
     private void OnDestroy() {
        if(_udpSocketManager != null) {
            _udpSocketManager.closeSocketThreads();
        }
    }
}
