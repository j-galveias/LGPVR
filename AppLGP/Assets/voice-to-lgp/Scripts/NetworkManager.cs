using Oculus.Interaction;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Oculus.Interaction.InteractableColorVisual;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public RawImage img;
    bool alreadyConnected = false;
	public InteractableColorVisual colorButton;
	// Start is called before the first frame update
	void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
		Debug.Log("-------------");
		foreach(var a in PhotonNetwork.PlayerList)
        {
			Debug.Log(a.ActorNumber);
        }
        /*if (PhotonNetwork.IsConnected && !alreadyConnected)
        {
            img.color = Color.green;
            alreadyConnected = true;
        }*/
    }

	#region MonoBehaviourPunCallbacks CallBacks
	// below, we implement some callbacks of PUN
	// you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


	/// <summary>
	/// Called after the connection to the master is established and authenticated
	/// </summary>
	public override void OnConnectedToMaster()
	{
		// we don't want to do anything if we are not attempting to join a room. 
		// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
		// we don't want to do anything.
		
		//PhotonNetwork.AutomaticallySyncScene = true;
		RoomOptions options = new RoomOptions()
		{
			IsVisible = true,
			IsOpen = true,
			MaxPlayers = (byte)2
		};
		PhotonNetwork.JoinOrCreateRoom("LGPvr", options, null);
	}

	/// <summary>
	/// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
	/// </summary>
	/// <remarks>
	/// Most likely all rooms are full or no rooms are available. <br/>
	/// </remarks>
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
	}


	/// <summary>
	/// Called after disconnecting from the Photon server.
	/// </summary>
	public override void OnDisconnected(DisconnectCause cause)
	{
		
		Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");
		if(colorButton != null)
        {
			ColorState c = new ColorState();
			c.Color = Color.red;
			c.ColorTime = 0.1f;
			colorButton.InjectOptionalNormalColorState(c);
			c = new ColorState();
			c.Color = new Color(1, 0, 0, 115f / 255f);
			c.ColorTime = 0.1f;
			colorButton.InjectOptionalHoverColorState(c);
			c = new ColorState();
			c.Color = new Color(1, 0, 0, 36f / 255f);
			c.ColorTime = 0.05f;
			colorButton.InjectOptionalSelectColorState(c);
			colorButton.UpdateVisual();
        }
	}

	/// <summary>
	/// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
	/// </summary>
	/// <remarks>
	/// This method is commonly used to instantiate player characters.
	/// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
	///
	/// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
	/// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
	/// enough players are in the room to start playing.
	/// </remarks>
	public override void OnJoinedRoom()
	{
		Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
		//img.color = Color.green;
		if (colorButton != null)
		{
			ColorState c = new ColorState();
			c.Color = Color.green;
			c.ColorTime = 0.1f;
			colorButton.InjectOptionalNormalColorState(c);
			c = new ColorState();
			c.Color = new Color(0, 1, 0, 115f / 255f);
			c.ColorTime = 0.1f;
			colorButton.InjectOptionalHoverColorState(c);
			c = new ColorState();
			c.Color = new Color(0, 1, 0, 36f / 255f);
			c.ColorTime = 0.05f;
			colorButton.InjectOptionalSelectColorState(c);
			colorButton.UpdateVisual();
		}
	}

	#endregion
}
