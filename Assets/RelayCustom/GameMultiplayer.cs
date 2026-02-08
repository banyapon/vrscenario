using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;

public class GameMultiplayer : MonoBehaviour
{
    [SerializeField] bool m_CreateTestPrimitives = true;

    // กำหนดค่า Default ผู้เล่นสูงสุดไว้ที่นี่
    private const int DefaultMaxPlayers = 16; 

    void Start()
    {
        if (!m_CreateTestPrimitives)
            return;

        CreateTestPrimitive(PrimitiveType.Plane, "TestPlane", Vector3.zero, new Vector3(5f, 1f, 5f));
        CreateTestPrimitive(PrimitiveType.Cube, "TestCube", new Vector3(0f, 0.5f, 0f), Vector3.one);
    }

    public async Task<string> Create16PlayerSession()
    {
        try
        {
            // ตั้งค่า Options สำหรับ Session
            var options = new SessionOptions
            {
                Name = "sixteenroom", 
                MaxPlayers = DefaultMaxPlayers,
                IsPrivate = false,
                IsLocked = false
            };
            
            // เพิ่มฟีเจอร์ Relay (เพื่อให้คนอื่นจอยผ่านเน็ตได้)
            // และ Lobby (เพื่อให้ห้องนี้ค้นหาเจอในหน้า Lobby)
            options.WithRelayNetwork();

            // สร้าง Session
            var session = await MultiplayerService.Instance.CreateSessionAsync(options);
            
            Debug.Log($"สร้างห้องสำเร็จ! รองรับผู้เล่น: {session.MaxPlayers} คน");
            Debug.Log($"Join Code: {session.Code}");
            
            return session.Code;
        }
        catch (SessionException e)
        {
            Debug.LogError($"สร้างห้องไม่สำเร็จ: {e.Message}");
            return null;
        }
    }
    static void CreateTestPrimitive(PrimitiveType type, string name, Vector3 position, Vector3 localScale)
    {
        if (GameObject.Find(name) != null)
            return;

        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = localScale;
    }
}
