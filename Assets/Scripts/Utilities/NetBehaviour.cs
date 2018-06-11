﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Special class for objects that should be
// controlled by specific clients over the network
public abstract class NetBehaviour : NetworkBehaviour 
{
	#region DATA
	// When spawned, if local authority is set on 
	private static Dictionary<Type, NetBehaviour> localInstances
			 = new Dictionary<Type, NetBehaviour> ();

	// Network-shared name
	[SyncVar (hook = "UpdateName")]
	private string netName;

	internal NetworkIdentity id;
	#endregion

	#region CALLBACKS
	private void Awake () 
	{
		id = GetComponent<NetworkIdentity> ();
		OnAwake ();
	}
	protected virtual void OnAwake () { }
	#endregion

	#region HELPERS
	[Server] public void SetName (string name) 
	{
		UpdateName (name);
	}

	private void UpdateName (string name) 
	{
		if (isClient) 
		{
			if (hasAuthority)	name = name.Insert (0, "[OWN] ");
			else				name = name.Insert (0, "[OTHER] ");
			name = name.Insert (0, "["+connectionToServer.connectionId+"]");
		}
		else
		if (isServer) 
		{
			if (!id.serverOnly)
			{
				name = name.Insert (0, "[CLIENT] ");

				var o = id.clientAuthorityOwner;
				if (o != null) name = name.Insert (0, "["+o.connectionId+"]");
			}
			else name = name.Insert (0, "[SERVER] ");
		}

		// Show on inspector
		this.name = netName = name;
	}

	// Tries to add this Instance to the dictionary
	[Client] private void AddToDictionary () 
	{
		var type = GetType ();
		if (localInstances.ContainsKey (type))
		{
			if (Debug.isDebugBuild)
				Debug.LogWarning ("Already exists an instance for type " + type + "!");
			return;
		}
		else localInstances.Add (type, this);
	}

	// Returns the scene object that has local Player authority of given type
	[Client] public static T GetLocal<T> () where T : NetBehaviour
	{
		NetBehaviour local;
		if (!localInstances.TryGetValue (typeof (T), out local))
		{
			if (Debug.isDebugBuild)
				Debug.LogWarning ("No local instance for method " + typeof (T) + " found.");
			return null;
		}
		else return local as T;
	}
	#endregion
}
