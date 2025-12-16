using UnityEngine;
using UnityEngine.Video;

public class TVController : MonoBehaviour
{
    [Header("Cài đặt chung")]
    public VideoPlayer videoPlayer;       // Component Video Player
    public MeshRenderer tvScreenRenderer; // Mesh của cái màn hình (để đổi màu)

    [Header("Vật liệu (Material)")]
    public Material videoMat; // Material chứa Render Texture (TV_Display_Mat)
    public Material offMat;   // Material màu đen bóng (BlackMat)

    private bool isTvOn = false;

    void Start()
    {
        // Khởi đầu game: Tắt TV
        TurnOffTV();
    }

    public void ToggleTV()
    {
        isTvOn = !isTvOn;
        if (isTvOn) TurnOnTV();
        else TurnOffTV();
    }

    void TurnOnTV()
    {
        if (tvScreenRenderer != null && videoMat != null)
        {
            // 1. Đổi áo cho màn hình thành loại hiển thị Video
            tvScreenRenderer.material = videoMat;
        }

        if (videoPlayer != null)
        {
            videoPlayer.enabled = true;
            videoPlayer.Play();
        }
    }

    void TurnOffTV()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.enabled = false;
        }

        if (tvScreenRenderer != null && offMat != null)
        {
            // 2. Đổi áo cho màn hình về màu đen bóng
            tvScreenRenderer.material = offMat;
        }
    }
}