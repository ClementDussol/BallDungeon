﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeController
{
	public static void SetTimeScale(float timeScale)
	{
		Time.timeScale = timeScale;
		Time.fixedDeltaTime = 0.02f * timeScale;
	}
}
