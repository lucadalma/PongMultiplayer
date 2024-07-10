using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerColourSetter : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteColour = new SpriteRenderer();

    [SyncVar(hook = nameof(HandleTeamColourUpdated))]
    private Color teamColour = new Color();

    #region Server

    public override void OnStartServer()
    {
        PongPlayer player = connectionToClient.identity.GetComponent<PongPlayer>();
        teamColour = player.TeamColour;
    }

    #endregion

    
    #region Client

    private void HandleTeamColourUpdated(Color oldColour, Color newColour)
    {
        spriteColour.color = newColour;
    }

    #endregion
}
