using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class Read_Input : MonoBehaviour
{
    public bool useAngle = true;
    public bool useLocation = true;
    public Vector3 location_weights = new Vector3(1f, 1f, 1f);
    public Vector3 angle_weights = new Vector3(1f, 1f, 1f);
    public int PORT = 7112;
    private Vector3 target_location;
    private Vector3 target_angle;
    private UdpClient udpClient;
    private CharacterController controller; 
    private IPEndPoint groupEP;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        

        udpClient = new UdpClient(PORT);
        groupEP = new IPEndPoint(IPAddress.None, PORT);
        // udpClient.Client.Bind(groupEP);
        //udpClient.JoinMulticastGroup(IPAddress.Any);
        target_location = transform.position;
        target_angle = transform.rotation.eulerAngles; 

        // offset = transform.position;
    }

    async void ReadTargets(){
        await Task.Run(() => {
            byte[] bytes;
            try{
                bytes = udpClient.Receive(ref groupEP);
                //Debug.Log($"x: {BitConverter.ToSingle( bytes, 0 )} y: {BitConverter.ToSingle( bytes, 4 )} z: {BitConverter.ToSingle( bytes, 8)}");
                if (useLocation){
                    var temp = new Vector3(BitConverter.ToSingle( bytes, 8),
                    BitConverter.ToSingle( bytes, 4 ), 
                    BitConverter.ToSingle( bytes, 0 ));
                    target_location =  Vector3.Scale(target_location, location_weights);
                }
                if (useAngle){
                    var temp = new Vector3(BitConverter.ToSingle( bytes, 16 ),
                    BitConverter.ToSingle( bytes, 20 ),
                    BitConverter.ToSingle( bytes, 12)
                    // 
                    );
                    target_angle = Vector3.Scale(temp, angle_weights);
                }
                
                
            } 
            catch{
                Debug.Log("failed");
            } 
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        this.ReadTargets();
        if (useLocation)
        {
            transform.position = (target_location + offset);
        }
        if (useAngle) 
        {
            // transform.Rotate(target_angle);
            // transform.localEulerAngles = target_angle;
            // transform.eulerAngles = target_angle;
            transform.Rotate(target_angle - transform.rotation.eulerAngles, Space.Self);
        }
        
    }
}
