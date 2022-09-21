using UnityEngine;
using UnityEngine.InputSystem;

public class VisualizedBlocks : MonoBehaviour {

    public AudioSource music;

    public GameObject blockPrefab;
    public int count = 21, sampleWidth = 10;
    public float maxHeight = 2f, width = 0.3f;

    private readonly int sampleCount = 1024;
    private LoopingMusic loopingMusic;
    private float[] samples;
    private GameObject[] blocks;
    private bool playing;

    public void Start() {
        samples = new float[sampleCount];
        blocks = new GameObject[count];
        loopingMusic = GetComponent<LoopingMusic>();

        for (int i = 0; i < count; i++) {
            blocks[i] = Instantiate(blockPrefab,transform);
            blocks[i].transform.localPosition = new((width + 0.1f) * i, 0);
        }

        loopingMusic.Stop();
    }

    // Update is called once per frame
    public void Update() {
        music.GetSpectrumData(samples, 0, FFTWindow.Rectangular);
        for (int i = 0; i < count; i++) {

            float hertz = i * 20000f / count;
            int index = 1024 - (int) (Mathf.Log(20000-hertz, 2) / Mathf.Log(20000, 2) * sampleCount);

            float total = 0;
            int count2 = 0;
            for (int j = -10; j <= 10; j++) {
                if (index + j < 0 || index + j >= 1024)
                    continue;

                total += samples[index + j];
                count2++;
            }

            blocks[i].transform.localScale = new(width, (total/count2) * maxHeight, 1);
        }

        if (Keyboard.current[Key.H].wasPressedThisFrame) {
            if (playing) {
                loopingMusic.Stop();
            } else {
                loopingMusic.Play(true);
            }
            playing = !playing;
        }
    }

    public void OnDrawGizmos() {
        Vector3 size = new Vector3(width, maxHeight, 1);
        for (int i = 0; i < count; i++) {
            Gizmos.DrawCube(transform.position + new Vector3((width + 0.1f) * i, 0) + (size.Multiply(Vector3.up) * 0.5f), size);
        }
    }
}
