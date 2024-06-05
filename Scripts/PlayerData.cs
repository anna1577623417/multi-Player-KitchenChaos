using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData :IEquatable<PlayerData>,INetworkSerializable {

    public ulong clientId;
    public int colorId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    public bool Equals(PlayerData other) {
        return clientId == other.clientId && 
            colorId==other.colorId&&
            playerName==other.playerName&&
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
        //ref是C#中的关键字，用于将变量传递给方法时，按引用(reference)传递而不是按值传递。使用ref关键字可以使方法修改传递的变量的值
        //使得函数可以修改参数的值
        //类似传入变量的指针
    }
}
