using UnityEngine;
using UnityEngine.UIElements;

public class CameraFitter : MonoBehaviour
{
    public SpriteRenderer background;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // 배경 이미지의 크기
        float bgHeight = background.bounds.size.y;
        // 카메라가 비추는 높이의 길이
        float camHeight = cam.orthographicSize * 2;
        // 카메라의 세로 사이즈 (Orthographic Size)를 배경 높이에 맞추기
        cam.orthographicSize = bgHeight / 2;
    }
}
