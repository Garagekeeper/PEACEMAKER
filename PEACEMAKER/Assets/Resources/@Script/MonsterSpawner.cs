using System;
using System.Collections;
using System.Collections.Generic;
using Resources.Script;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [Header("설정")]
    //TODO 난이도에 따라서 수정할 영역
    [SerializeField] private Transform playerTransform; // 플레이어 위치
    [SerializeField] private float spawnRadius = 30f;   // 소환 반경
    [SerializeField] private float spawnInterval = 0.5f; // 소환 간격 (초)
    [SerializeField] private int maxMonsterCount = 200;  // 최대 몬스터 수
    
    private float timer;

    private void Awake()
    {
        playerTransform = HeadManager.Game.MainPlayer.transform;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 1. 소환 간격 체크 && 2. 최대 개수 체크
        if (timer >= spawnInterval && HeadManager.ObjManager.Enemies.Count < maxMonsterCount)
        {
            SpawnMonster();
            timer = 0f;
        }
    }

    void SpawnMonster()
    {
        if (playerTransform == null) return;

        // 3. 플레이어 기준 반경 내 랜덤 좌표 계산
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = new Vector3(
            playerTransform.position.x + randomCircle.x,
            playerTransform.position.y, // 지형 높이에 맞게 조정 필요시 수정
            playerTransform.position.z + randomCircle.y
        );
        
        // 몬스터 생성
        var mon = HeadManager.ObjManager.Spawn<Enemy>(Defines.EObjectID.Enemy, spawnPos);
    }

    // 에디터 뷰에서 소환 범위를 시각적으로 확인
    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, spawnRadius);
        }
    }
}