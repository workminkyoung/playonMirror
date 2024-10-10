using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class CouponValidataResponse
{
    public bool is_used;
    public bool is_expired;
    public bool is_matched_uuid;
    public bool is_fixed_rate;
    public int amount;
    public void InitData()
    {
        is_used = true;
        is_expired = true;
        is_matched_uuid = true;
        is_fixed_rate = true;
        amount = 0; 
    }   
}       