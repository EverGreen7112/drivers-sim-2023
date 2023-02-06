using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubesAndConesLocator : MonoBehaviour
{
    private static UdpSocketManager _udpSocketManager;
    private static bool _isListenPortLogged = false;
    private static Vector3[] _cubes = new Vector3[]{new Vector3(0,0,0)};
    private static Vector3[] _cones = new Vector3[]{new Vector3(0,0,0)};
    public GameObject cone;
    public GameObject cube;
    public int PORT = 4590; // cant be this in actual competition
    public Transform offset;
    private List<GameObject> cones = new List<GameObject>{};
    private List<GameObject> cubes = new List<GameObject>{};
    // private cubesAndCones _cubesAndCones;
    void Start()
    {
        // _cubesAndCones = new cubesAndCones("127.0.0.1", PORT, PORT);
        _udpSocketManager = new UdpSocketManager("127.0.0.1", PORT, PORT);
        StartCoroutine(_udpSocketManager.initSocket());
        StartCoroutine(receiveStream());
        if (cube == null) {
            cube = transform.GetChild(0).gameObject;
        }
        if (cone == null) {
            cone = transform.GetChild(1).gameObject;
        }
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
                Debug.Log("Finished reading cones");
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


    // Update is called once per frame
    private void updateConesAndCubes(){
        // // cones
        // for (int i = 0;  i < cubesAndCones.Cones.Length; i++) {
        //     if (cones.Count <= i) {
        //         cones.Add(GameObject.Instantiate(cone));
        //     }
        //     cones[i].transform.position = offset.position + cubesAndCones.Cones[i];
        // }
        // if (cubesAndCones.Cones.Length < cones.Count) {
        //     for (int i = cubesAndCones.Cones.Length;  i < cones.Count; i++) {
        //         Destroy(cones[i]);
        //     }
        // }
        // // cubes
        // for (int i = 0;  i < cubesAndCones.Cubes.Length; i++) {
        //     if (cubes.Count <= i) {
        //         cubes.Add(GameObject.Instantiate(cube));
        //     }
        //     cubes[i].transform.position = offset.position + cubesAndCones.Cubes[i];
        // }
        // if (cubesAndCones.Cubes.Length < cubes.Count) {
        //     for (int i = cubesAndCones.Cubes.Length;  i < cubes.Count; i++) {
        //         Destroy(cubes[i]);
        //     }
        // }
        // cones
        for (int i = 0;  i < _cones.Length; i++) {
            if (cones.Count <= i) {
                cones.Add(GameObject.Instantiate(cone));
            } else if (cones[i] == null) {
                cones[i] = GameObject.Instantiate(cone);
            }
            cones[i].transform.position = offset.position + _cones[i];

        }
        if (_cones.Length < cones.Count) {
            for (int i = _cones.Length; i < cones.Count; i++) {
                Destroy(cones[i]);
                cones[i] = null;
            }
        }
        // cubes
        for (int i = 0;  i < _cubes.Length; i++) {
            if (cubes.Count <= i) {
                cubes.Add(GameObject.Instantiate(cube));
            } else if (cubes[i] == null) {
                cubes[i] = GameObject.Instantiate(cube);
            }
            cubes[i].transform.position = offset.position + _cubes[i];
        }
        if (_cubes.Length < cubes.Count) {
            for (int i = _cubes.Length; i < cubes.Count; i++) {
                Destroy(cubes[i]);
                cubes[i] = null;
            }
        }

    }
    void Update()
    {
        updateConesAndCubes();
    }



    private void OnDestroy() {
        if(_udpSocketManager != null) {
            _udpSocketManager.closeSocketThreads();
        }
    }
}
