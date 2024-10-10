using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouponValidataResponse
{
    public bool is_used;
    public bool is_expired;
    public bool is_matched_uuid;
    public bool is_fixed_rate;
    public int amount;
}

public class CouponValidataResponse422
{
    public string detail;
}