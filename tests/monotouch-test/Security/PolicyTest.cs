//
// SecPolicy Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using Security;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.Security;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Security {
	
	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class SecPolicyTest {

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		[Test]
		public void SslServerNoHost ()
		{
			using (var policy = SecPolicy.CreateSslPolicy (true, null)) {
				Assert.That (policy.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (CFGetRetainCount (policy.Handle), Is.EqualTo ((nint) 1), "RetainCount");

				if (TestRuntime.CheckSystemAndSDKVersion (7,0)) {
					using (var properties = policy.GetProperties ()) {
						Assert.That (properties.Handle, Is.Not.EqualTo (IntPtr.Zero), "Properties.Handle");
						Assert.That (CFGetRetainCount (properties.Handle), Is.EqualTo ((nint) 1), "Properties.RetainCount");
						Assert.That (properties.Count, Is.EqualTo ((nuint) 1), "Count");
						Assert.That (properties [SecPolicyPropertyKey.Oid].ToString (), Is.EqualTo ("1.2.840.113635.100.1.3"), "SecPolicyOid");
					}
				}
			}
		}

		[Test]
		public void SslServer ()
		{
			using (var policy = SecPolicy.CreateSslPolicy (true, "google.com")) {
				Assert.That (policy.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (CFGetRetainCount (policy.Handle), Is.EqualTo ((nint) 1), "RetainCount");

				if (TestRuntime.CheckSystemAndSDKVersion (7,0)) {
					using (var properties = policy.GetProperties ()) {
						Assert.That (properties.Handle, Is.Not.EqualTo (IntPtr.Zero), "Properties.Handle");
						Assert.That (CFGetRetainCount (properties.Handle), Is.EqualTo ((nint) 1), "Properties.RetainCount");
						Assert.That (properties.Count, Is.EqualTo ((nuint) 2), "Count");
						Assert.That (properties [SecPolicyPropertyKey.Oid].ToString (), Is.EqualTo ("1.2.840.113635.100.1.3"), "SecPolicyOid");
						Assert.That (properties [SecPolicyPropertyKey.Name].ToString (), Is.EqualTo ("google.com"), "SecPolicyName");
					}
				}
			}
		}

		[Test]
		public void SslClient ()
		{
			using (var policy = SecPolicy.CreateSslPolicy (false, null)) {
				Assert.That (policy.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (CFGetRetainCount (policy.Handle), Is.EqualTo ((nint) 1), "RetainCount");

				if (TestRuntime.CheckSystemAndSDKVersion (7,0)) {
					using (var properties = policy.GetProperties ()) {
						Assert.That (properties.Handle, Is.Not.EqualTo (IntPtr.Zero), "Properties.Handle");
						Assert.That (CFGetRetainCount (properties.Handle), Is.EqualTo ((nint) 1), "Properties.RetainCount");
						Assert.That (properties.Count, Is.EqualTo ((nuint) 2), "Count");
						Assert.That (properties [SecPolicyPropertyKey.Oid].ToString (), Is.EqualTo ("1.2.840.113635.100.1.3"), "SecPolicyOid");
						Assert.That (properties [SecPolicyPropertyKey.Client].ToString (), Is.EqualTo ("1"), "SecPolicyClient");
					}
				}
			}
		}

		[Test]
		public void BasicX509Policy ()
		{
			using (var policy = SecPolicy.CreateBasicX509Policy ()) {
				Assert.That (policy.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (CFGetRetainCount (policy.Handle), Is.EqualTo ((nint) 1), "RetainCount");

				if (TestRuntime.CheckSystemAndSDKVersion (7,0)) {
					using (var properties = policy.GetProperties ()) {
						Assert.That (properties.Handle, Is.Not.EqualTo (IntPtr.Zero), "Properties.Handle");
						Assert.That (CFGetRetainCount (properties.Handle), Is.EqualTo ((nint) 1), "Properties.RetainCount");
						Assert.That (properties.Count, Is.EqualTo ((nuint) 1), "Count");
						Assert.That (properties [SecPolicyPropertyKey.Oid].ToString (), Is.EqualTo ("1.2.840.113635.100.1.2"), "SecPolicyOid");
					}
				}
			}
		}

		[Test]
		public void RevocationPolicy ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS7");

			using (var policy = SecPolicy.CreateRevocationPolicy (SecRevocation.UseAnyAvailableMethod | SecRevocation.RequirePositiveResponse)) {
				Assert.That (policy.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (CFGetRetainCount (policy.Handle), Is.EqualTo ((nint) 1), "RetainCount");

				using (var properties = policy.GetProperties ()) {
					Assert.That (properties.Handle, Is.Not.EqualTo (IntPtr.Zero), "Properties.Handle");
					Assert.That (CFGetRetainCount (properties.Handle), Is.EqualTo ((nint) 1), "Properties.RetainCount");
					Assert.That (properties.Count, Is.EqualTo ((nuint) 1), "Count");
					Assert.That (properties [SecPolicyPropertyKey.Oid].ToString (), Is.EqualTo ("1.2.840.113635.100.1.21"), "SecPolicyOid");
				}
			}
		}

		void CreatePolicy (NSString oid)
		{
			string name = oid + ".";
			using (var policy = SecPolicy.CreatePolicy (oid, null)) {
				Assert.That (CFGetRetainCount (policy.Handle), Is.EqualTo ((nint) 1), name + "RetainCount");
				Assert.That (policy.GetProperties ().Values [0], Is.EqualTo (oid), name + "SecPolicyOid");
			}
		}

		[Test]
		public void CreatWellKnownPolicies ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS7");

			CreatePolicy (SecPolicyIdentifier.AppleX509Basic);
			CreatePolicy (SecPolicyIdentifier.AppleSSL);
			CreatePolicy (SecPolicyIdentifier.AppleSMIME);
			// crash
			// CreatePolicy (SecPolicyIdentifier.AppleEAP);
			CreatePolicy (SecPolicyIdentifier.AppleIPsec);
			CreatePolicy (SecPolicyIdentifier.AppleCodeSigning);
			CreatePolicy (SecPolicyIdentifier.AppleIDValidation);
			// invalid handle ? not yet supported ?!?
			// CreatePolicy (SecPolicyIdentifier.AppleTimeStamping);
			CreatePolicy (SecPolicyIdentifier.AppleRevocation);
		}

		[Test]
		public void CreateUnknownPolicy ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS7");

			using (var oid = new NSString ("1.2.3.4")) {
				Assert.Throws<ArgumentException> (delegate {
					SecPolicy.CreatePolicy (oid, null);
				});
			}
		}
	}
}
