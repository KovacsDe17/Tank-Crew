using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMutliplayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;
    bool spawned = false;

    private Rigidbody2D rigidbody;
    NetworkVariable<MyCustomData> count = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            _int = 42,
            _bool = true,
            _nickname = "N/A"
        },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );


    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes _nickname;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _nickname);
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButton(0))
        {
            rigidbody.AddTorque(-2f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            SpawnOrDespawn();

            /*
            //Send the message only to the client with the ID: 1
            TestClientRPC(new ClientRpcParams {Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } });

            //Update MyCustomData
            count.Value = new MyCustomData {
                _int = 16,
                _bool = false,
                _nickname = OwnerClientId == 0 ? "DriverMan" : "GunnerGuy"
            };
            */
        }

    }

    private void SpawnOrDespawn()
    {
        if (!spawned)
        {
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        }
        else
        {
            Destroy(spawnedObjectTransform.gameObject);
        }

        spawned = !spawned;
    }

    //Executed when the object is spawned
    public override void OnNetworkSpawn()
    {
        //Subscribe to the "change of value" event
        count.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            //Log only on value change  
            Debug.Log(newValue._nickname + " (" + OwnerClientId + "); Count: {" + newValue._int + "; " + newValue._bool + "}");
        };
    }

    [ServerRpc]
    public void TestServerRPC(ServerRpcParams serverRpcParams)
    {
        Debug.Log("TestServerRPC " + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void TestClientRPC(ClientRpcParams clientRpcParams)
    {
        Debug.Log("TestClientRPC");
    }
}
