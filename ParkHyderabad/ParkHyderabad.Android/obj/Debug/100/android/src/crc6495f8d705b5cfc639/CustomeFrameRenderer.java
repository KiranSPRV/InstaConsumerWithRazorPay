package crc6495f8d705b5cfc639;


public class CustomeFrameRenderer
	extends crc64720bb2db43a66fe9.FrameRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("ParkHyderabad.Droid.CustomeFrameRenderer, ParkHyderabad.Android", CustomeFrameRenderer.class, __md_methods);
	}


	public CustomeFrameRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == CustomeFrameRenderer.class)
			mono.android.TypeManager.Activate ("ParkHyderabad.Droid.CustomeFrameRenderer, ParkHyderabad.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public CustomeFrameRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == CustomeFrameRenderer.class)
			mono.android.TypeManager.Activate ("ParkHyderabad.Droid.CustomeFrameRenderer, ParkHyderabad.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public CustomeFrameRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == CustomeFrameRenderer.class)
			mono.android.TypeManager.Activate ("ParkHyderabad.Droid.CustomeFrameRenderer, ParkHyderabad.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
