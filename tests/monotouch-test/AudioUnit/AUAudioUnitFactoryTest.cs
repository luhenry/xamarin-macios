﻿//
// Unit tests for AUAudioUnitFactory
//
// Authors:
//	Oleg Demchenko (oleg.demchenko@xamarin.com)
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && XAMCORE_2_0

using System;

using NUnit.Framework;

using Foundation;
using AudioUnit;

namespace MonoTouchFixtures.AudioUnit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AUAudioUnitFactoryTest {
		[Test]
		public void CreateAudioUnit ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (9, 0))
				Assert.Ignore ("Ignoring AudioUnitv3 tests: Requires iOS9+");

			const string expectedManufacturer = "Apple";
			var desc = new AudioComponentDescription {
				ComponentType = AudioComponentType.Output,
				ComponentSubType = 0x72696f63, // Remote_IO
				ComponentManufacturer = AudioComponentManufacturerType.Apple
			};

			using (var auFactory = new CustomAudioUnitFactory ()) {
				NSError error;
				using (var audioUnit = auFactory.CreateAudioUnit (desc, out error)) {
					Assert.True (audioUnit != null, "CustomAudioUnitFactory returned null object for valid component description");
					Assert.True (audioUnit.ManufacturerName == expectedManufacturer,
						$"CustomAudioUnitFactory returned audio unit with incorrect manufacturer. Expected - {expectedManufacturer}, actual - {audioUnit.ManufacturerName}");
				}
			}
		}

		public class CustomAudioUnitFactory : NSObject, IAUAudioUnitFactory {
			public AUAudioUnit CreateAudioUnit (AudioComponentDescription desc, out NSError error)
			{
				var audioUnit = new AUAudioUnit (desc, out error);
				return audioUnit;
			}

			public void BeginRequestWithExtensionContext (NSExtensionContext context)
			{
				throw new NotImplementedException ();
			}
		}
	}
}

#endif // !__WATCHOS__ && XAMCORE_2_0
