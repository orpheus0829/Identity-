using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class Start_Hall_Camera : Base_Mgr<Start_Hall_Camera>
{
    [Header("×·×ÙÄ¿±ê")]
    public List<Transform> targets = new List<Transform>();

    [Header("¸úËæÉèÖÃ")]
    public float smoothTime = 0.3f;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float minZoom = 4f;
    public float maxZoom = 20f;
    public float zoomSmooth = 0.5f;

    [Header("ÈºÌåÅÐ¶¨·¶Î§")]
    public float groupRadius = 4f;

    [Header("¾²Ö¹ÅÐ¶¨")]
    public float moveThreshold = 0.01f;

    [Header("±ßÔµÁô°×")]
    public float padding = 1f;

    public Vector3 velocity;
    public Camera cam;
    public Dictionary<Transform, Vector3> lastPositions = new Dictionary<Transform, Vector3>();

    protected override void Awake()
    {
        base.Awake();
        cam = GetComponent<Camera>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            targets.Add(player.transform);
        }

        GameObject bot = GameObject.FindGameObjectWithTag("Butcher_Bot");
        if (bot != null)
        {
            targets.Add(bot.transform);
        }
    }

    public void LateUpdate()
    {
        targets.RemoveAll(RemoveNullTarget);

        if (targets.Count == 0)
        {
            return;
        }

        List<Transform> activeTargets = GetActiveMovingTargets();

        if (activeTargets.Count == 0)
        {
            return;
        }

        List<Transform> maxGroup = GetMaxCountGroup(activeTargets);
        List<Transform> allTarget = new List<Transform>(activeTargets);

        if (maxGroup.Count * 2 > allTarget.Count)
        {
            Vector3 groupCenter = CalculateCenterPoint(maxGroup);
            float requiredSize = CalculateRequiredSize(maxGroup);
            AdjustCameraView(groupCenter, requiredSize);
        }
        else
        {
            Vector3 allCenter = CalculateCenterPoint(allTarget);
            float requiredSize = CalculateRequiredSize(allTarget);
            AdjustCameraView(allCenter, requiredSize);
        }

        UpdateLastPositions(activeTargets);
    }

    public bool RemoveNullTarget(Transform target)
    {
        if (target == null)
        {
            return true;
        }
        return false;
    }

    public List<Transform> GetActiveMovingTargets()
    {
        List<Transform> movingList = new List<Transform>();

        for (int i = 0; i < targets.Count; i++)
        {
            Transform t = targets[i];

            if (t == null)
            {
                continue;
            }

            if (IsTargetMoving(t) == true)
            {
                movingList.Add(t);
            }
        }

        return movingList;
    }

    public bool IsTargetMoving(Transform target)
    {
        if (lastPositions.ContainsKey(target) == false)
        {
            return true;
        }

        float delta = Vector3.Distance(target.position, lastPositions[target]);

        if (delta > moveThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateLastPositions(List<Transform> movingTargets)
    {
        for (int i = 0; i < movingTargets.Count; i++)
        {
            Transform t = movingTargets[i];

            if (lastPositions.ContainsKey(t) == true)
            {
                lastPositions[t] = t.position;
            }
            else
            {
                lastPositions.Add(t, t.position);
            }
        }
    }

    public List<Transform> GetMaxCountGroup(List<Transform> currentTargets)
    {
        List<Transform> maxGroup = new List<Transform>();
        int maxMember = 0;

        for (int i = 0; i < currentTargets.Count; i++)
        {
            Transform currentTarget = currentTargets[i];
            if (currentTarget == null)
            {
                continue;
            }

            List<Transform> tempGroup = new List<Transform>();
            tempGroup.Add(currentTarget);

            for (int j = 0; j < currentTargets.Count; j++)
            {
                Transform checkTarget = currentTargets[j];
                if (checkTarget == null)
                {
                    continue;
                }

                float distance = Vector2.Distance(currentTarget.position, checkTarget.position);
                if (distance <= groupRadius)
                {
                    tempGroup.Add(checkTarget);
                }
            }

            if (tempGroup.Count > maxMember)
            {
                maxMember = tempGroup.Count;
                maxGroup = tempGroup;
            }
        }
        return maxGroup;
    }

    public Vector3 CalculateCenterPoint(List<Transform> group)
    {
        Vector3 centerPos = Vector3.zero;
        for (int i = 0; i < group.Count; i++)
        {
            centerPos = centerPos + group[i].position;
        }
        centerPos = centerPos / group.Count;
        return centerPos;
    }

    public float CalculateRequiredSize(List<Transform> group)
    {
        if (group.Count == 1)
        {
            return minZoom;
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < group.Count; i++)
        {
            minX = Mathf.Min(minX, group[i].position.x);
            maxX = Mathf.Max(maxX, group[i].position.x);
            minY = Mathf.Min(minY, group[i].position.y);
            maxY = Mathf.Max(maxY, group[i].position.y);
        }

        float width = (maxX - minX) + padding * 2;
        float height = (maxY - minY) + padding * 2;

        float sizeByWidth = width / cam.aspect * 0.5f;
        float sizeByHeight = height * 0.5f;

        return Mathf.Max(sizeByWidth, sizeByHeight);
    }

    public void AdjustCameraView(Vector3 centerPos, float targetSize)
    {
        float finalSize = Mathf.Clamp(targetSize, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, finalSize, zoomSmooth * Time.deltaTime);

        Vector3 targetPos = centerPos + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    public bool IsVisable_InCamera(Vector2 world_pos)
    {
        Vector2 view = cam.WorldToViewportPoint(world_pos);
        if (view.x > 0 && view.x < 1 && view.y > 0 && view.y < 1)
        {
            return true;
        }
        return false;
    }
}