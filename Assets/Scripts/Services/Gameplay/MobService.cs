using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobService : MonoBehaviour
{
    private PacketPublisher packetPublisher;
    private LoginHandler loginHandler;
    private MapService mapService;
    private HashSet<GameObject> mobs = new HashSet<GameObject>();

    public GameObject mobPrefab;

    void Awake()
    {
        packetPublisher = FindObjectOfType<PacketPublisher>();
        loginHandler = FindObjectOfType<LoginHandler>();
        mapService = FindObjectOfType<MapService>();

        packetPublisher.Subscribe(typeof(MobInfo), ProcessMobInfo);
        loginHandler.SelfDisconnect += Unload;
        loginHandler.LoginSuccess += Unload;
    }

    private void ProcessMobInfo(Message message)
    {
        MobInfo mobInfo = message as MobInfo;
        Vector3 position = new Vector3(mobInfo.Position.X, mapService.GetMapHeight(mobInfo.Position), mobInfo.Position.Y);
        mobs.Add(GameObject.Instantiate(mobPrefab, position, Quaternion.identity));
    }

    private void Unload()
    {
        List<GameObject> trash = new List<GameObject>();
        foreach(var mob in mobs)
        {
            trash.Add(mob);
        }
        foreach(var mob in trash)
        {
            GameObject.Destroy(mob);
            mobs.Remove(mob);
        }
    }
}
