using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draw : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject brush;
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject placeForPoints;
    [SerializeField] private GameObject anchorPoint;
    [SerializeField] private List<float> offset;
    [SerializeField] private float mouseOffset;

    public static float charactersOffset = 0.5f;

    private Vector3 mousePos;
    private Vector3[] pointToSpawn;
    private Vector3[] offsets = new Vector3[4]
{
        new Vector3 (charactersOffset, charactersOffset, 0),
        new Vector3(-charactersOffset, -charactersOffset, 0),
        new Vector3(-charactersOffset, charactersOffset, 0),
        new Vector3(charactersOffset, -charactersOffset, 0),
};

    private GameObject _line;
    private LineRenderer lineRenderer;

    private void OnEnable()
    {
        ProceduralTileGenerator.OnRetry += onRetry;
    }

    public void onRetry()
    {
        Tutorial tutor = GetComponent<Tutorial>();
        tutor.enabled = true;
    }

    private void FixedUpdate()
    {
        if (UIManager.Default.CurState == UIManager.State.MainMenu || UIManager.Default.CurState == UIManager.State.Play)
        {
            if (lineRenderer != null)
            {
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    if (i > offset.Count)
                        return;

                    lineRenderer.SetPosition(i, new Vector3(lineRenderer.GetPosition(i).x, lineRenderer.GetPosition(i).y, anchorPoint.transform.position.z + offset[i]));
                }
            }
        }
    }

    private void Update()
    {
        if (UIManager.Default.CurState == UIManager.State.Win || UIManager.Default.CurState == UIManager.State.Failed)
        {
            Destroy(_line);
        }
        if (UIManager.Default.CurState == UIManager.State.MainMenu || UIManager.Default.CurState == UIManager.State.Play)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("DrawLayer")))
                {
                    if (hit.collider.gameObject.layer == 8)
                    {
                        CreateBrush(hit.point);
                        Clear();
                    }
                }
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetMouseButton(0))
            {
                if ((Mathf.Abs(mousePos.x - Input.mousePosition.x) > mouseOffset || Mathf.Abs(mousePos.y - Input.mousePosition.y) > mouseOffset))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("DrawLayer")))
                    {
                        if (hit.collider.gameObject.layer == 8)
                        {
                            if (lineRenderer == null)
                            {
                                CreateBrush(hit.point);
                                Clear();
                            }

                            AddPoint(hit.point);
                            mousePos = Input.mousePosition;
                        }
                        else if (UIManager.Default.CurState == UIManager.State.Win)
                            Clear();
                        else
                        {
                            if (lineRenderer != null)
                            {
                                EndDraw();
                            }
                        }

                    }
                }
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
            {
                if (lineRenderer != null)
                    EndDraw();
            }
            else
            {
                lineRenderer = null;
            }
        }
    }

    private void CreateBrush(Vector3 poinPos)
    {
        GameObject brushInstantiate = Instantiate(brush);
        lineRenderer = brushInstantiate.GetComponent<LineRenderer>();
        _line = brushInstantiate;
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        lineRenderer.SetPosition(0, poinPos);
        lineRenderer.SetPosition(1, poinPos);
        offset.Add(lineRenderer.GetPosition(0).z - anchorPoint.transform.position.z);
        offset.Add(lineRenderer.GetPosition(1).z - anchorPoint.transform.position.z);
        LayerDefault.Default.playersFollowTargets.Clear();
    }

    private void AddPoint(Vector3 poinPos)
    {
        lineRenderer.positionCount++;
        int positionIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(positionIndex, poinPos);
        offset.Add(lineRenderer.GetPosition(positionIndex).z - anchorPoint.transform.position.z);
    }

    private void EndDraw()
    {
        placeForPoints.transform.localScale = Vector3.one;
        float step = 0;
        //float resultDist = 0;
        for (int i = 0; i < lineRenderer.positionCount - 2; i++)
        {
            step += Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
        }
        step = step / (LayerDefault.Default.players.Count * 1f);
        int j = 0;
        int k = 0;
        int g = 1;
        Vector3 pointToSpawnArr = lineRenderer.GetPosition(j);
        for (int i = 0; i < LayerDefault.Default.players.Count; i++)
        {


            float distance = Vector3.Distance(pointToSpawnArr, lineRenderer.GetPosition(j + 1));
            float complexDist = distance;
            while (complexDist < step)
            {

                g++;
                if ((j + g) > lineRenderer.positionCount - 1)
                {
                    Debug.Log("!!!!!!!!!!");
                    g--;
                    break;
                }

                complexDist += Vector3.Distance(lineRenderer.GetPosition(j + g - 1), lineRenderer.GetPosition(j + g));
                //Debug.Log(complexDist);

            }
            j += g - 1;
            g = 1;
            k = 1;

            float difference = complexDist - step;

            Vector3 minus = (lineRenderer.GetPosition(j + 1) - lineRenderer.GetPosition(j)).normalized * difference;
            pointToSpawnArr = lineRenderer.GetPosition(j + 1) - minus;
            // resultDist += complexDist - difference;
            GameObject points = Instantiate(point, pointToSpawnArr, Quaternion.identity, placeForPoints.transform);
            points.transform.localPosition = new Vector3(points.transform.localPosition.x, points.transform.localPosition.y, 0);

        }
        Destroy(_line);
        offset.Clear();
        placeForPoints.transform.rotation = Quaternion.Euler(90f, 0, 0);
        placeForPoints.transform.localScale = Vector3.one * 4.35f;
        LayerDefault.Default.pointCount = placeForPoints.transform.childCount;
        LayerDefault.Default.SetFollowTarget();
        // placeForPoints.transform.localScale = Vector3.one;
    }

    private void Clear()
    {
        for (int i = 0; i < placeForPoints.transform.childCount; i++)
        {
            Destroy(placeForPoints.transform.GetChild(i).gameObject);
        }
        placeForPoints.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnDestroy()
    {
        ProceduralTileGenerator.OnRetry -= onRetry;
    }
}
