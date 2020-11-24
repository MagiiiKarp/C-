using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    private void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();

        var players = FindObjectsOfType<Player>();

        foreach (var player in players)
        {
            player.OnCharacterChanged +=  (character) => Player_OnCharacterChanged(player, character);
        }
    }

    private void Player_OnCharacterChanged(Player player, Character character)
    {
        int playerIndex = player.PlayerNumber - 1;
        targetGroup.m_Targets[playerIndex].target = character.transform;
    }
}