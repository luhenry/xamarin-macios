﻿//
// Unit tests for HKCategoryTypeIdentifier
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;

#if XAMCORE_2_0
using Foundation;
using HealthKit;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.HealthKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CategoryTypeIdentifier {

		[Test]
		public void EnumValues_22351 ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Inconclusive ("Requires iOS8+");

			foreach (HKCategoryTypeIdentifier value in Enum.GetValues (typeof (HKCategoryTypeIdentifier))) {

				switch (value) {
				case HKCategoryTypeIdentifier.SleepAnalysis:
					break;
				default:
					if (!TestRuntime.CheckiOSSystemVersion (9, 0))
						continue;
					break;
				}

				try {
					using (var ct = HKCategoryType.Create (value)) {
						Assert.That (ct.Handle, Is.Not.EqualTo (IntPtr.Zero), value.ToString ());
					}
				}
				catch (Exception e) {
					Assert.Fail ("{0} could not be created: {1}", value, e);
				}
			}
		}
	}
}

#endif // !__TVOS__
