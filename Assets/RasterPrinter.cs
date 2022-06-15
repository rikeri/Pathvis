using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

public class RasterPrinter : MonoBehaviour
{
    public RasterPixels tracerCamera;
    public GameObject photoPrefab;
    public Transform photoSpawn;
    public Transform photoTarget;
    public Transform cameraOrigin; // used to align a raycast to detect which booth is photographed
    public Transform rasterBoard;
    private Throwable lastPhotoThrowable;
    public AudioClip buttonSound;
    public UnityEvent onSuccessfulPrint;

    public Transform BoothIdentifiers;

    private bool photoInPrinter = false;
    public void SpawnPhoto()
    {
        AudioSource.PlayClipAtPoint(buttonSound, transform.position, 0.5f);
        if (photoInPrinter) return; // user has to grab a photo before printing a new one

        string info = tracerCamera.StatsString();
        info = $"{tracerCamera.renderMode}\n{info}";

        Texture2D rasterTex = tracerCamera.PrintTexture();
        GameObject photo = Instantiate(photoPrefab, photoSpawn.position, photoSpawn.rotation);
        photo.transform.parent = photoSpawn.parent;
        photo.transform.localScale = photoSpawn.localScale;

        photo.GetComponent<MeshRenderer>().materials[0].mainTexture = rasterTex;
        photo.GetComponentInChildren<TextMesh>().text = info;

        lastPhotoThrowable = photo.GetComponent<Throwable>();
        lastPhotoThrowable.onPickUp.AddListener(PhotoGrabbed);
        photoInPrinter = true;
        StartCoroutine(MovePhoto(photo.transform));
        onSuccessfulPrint.Invoke();

        CheckAndNotifyBooth(rasterTex);
    }

    IEnumerator MovePhoto(Transform photoTransform) {
        Vector3 startPos = photoSpawn.localPosition;
        Vector3 endPos = photoTarget.localPosition;
        float moveTime = 1f;
        for (float t = 0; t < moveTime; t += Time.deltaTime)
        {
            Vector3 pos = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t/moveTime));
            photoTransform.localPosition = pos;
            yield return null;
        }

    }

    private void PhotoGrabbed()
    {
        StopAllCoroutines();
        photoInPrinter = false;
        lastPhotoThrowable.onPickUp.RemoveListener(PhotoGrabbed);
    }

    private void CheckAndNotifyBooth(Texture2D printedTexture)
    {
        // if RT camera is INSIDE a booth, it should count as a valid photo of it
        foreach (Transform booth in BoothIdentifiers)
        {
            BoxCollider collider = booth.GetComponent<BoxCollider>();
            if (collider.bounds.Contains(rasterBoard.position))
            {
                booth.GetComponent<PhotoableBooth>().BoothPhotographed(printedTexture);
                return;
            }
        }
        // if RT camera was not inside a booth, do a raycast in the booth identifier layer (12)
        RaycastHit boothHit;
        if (Physics.Raycast(cameraOrigin.position, (rasterBoard.position-cameraOrigin.position).normalized, out boothHit, 5, 1 << 12))
        {
            boothHit.collider.gameObject.GetComponent<PhotoableBooth>().BoothPhotographed(printedTexture);
            return;
        }
    }

}
