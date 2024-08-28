using MapGenerator;
using UnityEngine;


namespace ForBuildRoom
{
    public class RoomSpawner : MonoBehaviour
    {
        [SerializeField] Room roomPrefab; // The prefab to instantiate
        [SerializeField] string roomName;   

        [Space(10)]
        [SerializeField] int rows = 5; // Number of rows
        [SerializeField] int columns = 6; // Number of columns
        [SerializeField] float spacing = 20.0f; // Spacing between each room


        [ContextMenu("Generate Rooms")]
        void Generate()
        {
            if (roomPrefab == null)
            {
                Debug.LogError("Room prefab is not assigned!");
                return;
            }

            Vector3 startPosition = transform.position + new Vector3(-(columns - 1) * spacing / 2, (rows - 1) * spacing / 2, 0);

            int cnt = 1;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Vector3 position = startPosition+ new Vector3(j * spacing, -i * spacing, 0);
                    Room newRoom = Instantiate(roomPrefab, position, Quaternion.identity, transform);
                    newRoom.gameObject.name = $"{roomName} {cnt++}";
                }
            }
        }

    }
}