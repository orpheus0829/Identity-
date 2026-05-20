using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Gate_Controller : Base_Mgr<Gate_Controller>
{
    public Slider Gate_Slider;
    public bool Is_Coding_Gate;
    public event Action On_Gate_Start;
    public event Action On_Gate_Stop;
    public static event Action On_Gate_finish;
    public Tilemap Gate_Wall;
    public Tilemap Gate_Foundation;
    public bool Final;
    public bool Finish;
    public CapsuleCollider2D cp;
    protected override void Awake()
    {
        base.Awake();
        cp = GetComponent<CapsuleCollider2D>();
        Gate_Slider.value = 0;
        Gate_Slider.maxValue = 1;
        Gate_Slider.gameObject.SetActive(false);
    }
    public void OnEnable()
    {
        On_Gate_Start += Start_Coding;
    }
    public void OnDisable()
    {
        On_Gate_Start -= Start_Coding;
    }
    public void Open()
    {
        if (!Final)
        {
            return;
        }
        On_Gate_Start?.Invoke();
    }
    public void Start_Coding()
    {
        if (Is_Coding_Gate)
        {
            return;
        }
        Gate_Slider.gameObject.SetActive(true);
        Is_Coding_Gate = true;
        StartCoroutine(Decoding());
    }
    public IEnumerator Decoding()
    {
        while (Is_Coding_Gate)
        {
            Gate_Slider.value += Time.deltaTime * 0.5f / Gate_Slider.maxValue;
            if (Gate_Slider.value >= Gate_Slider.maxValue)
            {
                Clear_Gate();
                AstarPath.active.Scan();
                Gate_Slider.gameObject.SetActive(false);
                Finish = true;
                On_Gate_finish?.Invoke();
                //Stop();
                yield break;
            }
            yield return null;
        }
    }
    public void Clear_Gate()
    {
        cp.enabled = false;
        for (int i = -4; i <= 0; i++)
        {
            for (int j = -6; j <= -4; j++)
            {
                Gate_Wall.SetTile(new Vector3Int(i, j, 0), null);
                Gate_Foundation.SetTile(new Vector3Int(i, j, 0), null);
            }
        }
        Gate_Wall.RefreshAllTiles();
        Gate_Foundation.RefreshAllTiles();
        if (Gate_Wall.TryGetComponent<TilemapCollider2D>(out var col1))
        {
            col1.enabled = false;
            col1.enabled = true;
        }
        if (Gate_Foundation.TryGetComponent<TilemapCollider2D>(out var col2))
        {
            col2.enabled = false;
            col2.enabled = true;
        }
    }
    #region ½»»¥
    public void Interact_Gate_Player_Real(Player pl)
    {
        if (Is_Coding_Gate)
        {
            return;
        }
        pl.rb.velocity = Vector2.zero;
        Open();
        return;
    }
    public void Interact_Gate_Player_AI(Player_AI ai)
    {
        if (ai.Player_Coding)
        {
            return;
        }
        if (Is_Coding_Gate)
        {
            return;
        }
        ai.Player_Coding = true;
        ai.CanControl = false;
        ai.rb.velocity = Vector2.zero;
        Open();
        return;
    }
    #endregion
}
