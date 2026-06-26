using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using UnityEngine.InputSystem;

public class PlayerStats : NetworkBehaviour
{
    public int m_ScorePerPress = 1;
    public int m_StartHealth = 100;

    //private readonly NetworkVariable<int> m_Score = new NetworkVariable<int>(
    //    0,
    //    NetworkVariableReadPermission.Everyone,
    //    NetworkVariableWritePermission.Owner);

    int m_Score;

    [Rpc(SendTo.Server)]
    private void ReqScoreRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        if (senderClientId != OwnerClientId)
        {
            return;
        }

        m_Score += m_ScorePerPress;

        BroadcastScoreRpc(m_Score);

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadcastScoreRpc(int newScore)
    {
        m_Score = newScore;
        ApplyScore(m_Score);
    }

    [Rpc(SendTo.Server)]
    private void RequestCurrentScoreRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        SendCurrentScoreRpc(m_Score, RpcTarget.Single(senderClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendCurrentScoreRpc(int currentScore, RpcParams rpcParams)
    {
        m_Score = currentScore;
        ApplyScore(m_Score);
    }

    private readonly NetworkVariable<int> m_Hp = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<FixedString32Bytes> m_DisplayName = new NetworkVariable<FixedString32Bytes>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {

        m_Hp.OnValueChanged += HandleHealthChanged;
        m_DisplayName.OnValueChanged += HandleNameChanged;

        ApplyHealth(m_Hp.Value);
        ApplyName(m_DisplayName.Value);

        if (IsServer)
        {
            m_Hp.Value = m_StartHealth;
        }

        if (IsOwner && m_DisplayName.Value.Length == 0)
        {
            m_DisplayName.Value = new FixedString32Bytes($"Player {OwnerClientId}");
        }

        if (!IsServer)
        {
            RequestCurrentScoreRpc();
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }


        if (keyboard.hKey.wasPressedThisFrame)
        {
            m_Hp.Value -= 5;
        }
    }

    public override void OnNetworkDespawn()
    {
        m_Hp.OnValueChanged -= HandleHealthChanged;
        m_DisplayName.OnValueChanged -= HandleNameChanged;
    }

    private void HandleScoreChanged(int prev, int current)
    {
        ApplyScore(current);
        Debug.Log($"[PlayerStats] {OwnerClientId} : 점수 변경 {prev} -> {current}");
    }

    private void HandleHealthChanged(int prev, int current)
    {
        ApplyHealth(current);
        Debug.Log($"[PlayerStats] {OwnerClientId} : HP 변경 {prev} -> {current}");
    }

    private void HandleNameChanged(FixedString32Bytes prev, FixedString32Bytes current)
    {
        ApplyName(current);
        Debug.Log($"[PlayerStats] {OwnerClientId} : 이름 변경 {prev} -> {current}");
    }

    private void ApplyScore(int value)
    {

    }

    private void ApplyHealth(int value)
    {

    }

    private void ApplyName(FixedString32Bytes value)
    {

    }
}
