using UnityEngine;
using UnityEngine.UIElements;

public class CameraFitter : MonoBehaviour
{
    public SpriteRenderer background;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // ��� �̹����� ũ��
        float bgHeight = background.bounds.size.y;
        // ī�޶� ���ߴ� ������ ����
        float camHeight = cam.orthographicSize * 2;
        // ī�޶��� ���� ������ (Orthographic Size)�� ��� ���̿� ���߱�
        cam.orthographicSize = bgHeight / 2;
    }
}
