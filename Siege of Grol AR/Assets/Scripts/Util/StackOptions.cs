﻿using System;

[Flags]
public enum StackOptions
{
    OVERLAY = 1,
    CLEARSTACK = 1 << 1,
    PUSHTOSTACK = 1 << 2,
}