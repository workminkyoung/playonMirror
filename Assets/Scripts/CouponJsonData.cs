using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class CouponValidataResponse
{
    public bool is_valid_number;
    public bool is_used;
    public bool is_active;
    public bool is_matched_uuid;
    public bool is_fixed_rate;
    public int amount;
    public void InitData()
    {
        is_valid_number = false;
        is_used = false;
        is_active = false;
        is_matched_uuid = false;
        is_fixed_rate = false;
        amount = 0; 
    }   
}