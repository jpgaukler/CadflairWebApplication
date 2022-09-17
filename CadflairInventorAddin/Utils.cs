using Inventor;
using System;
using System.Runtime.InteropServices;


namespace CadflairInventorAddin
{
    internal static class Globals
    {
        /// <summary>
        /// Global Inventor Application object.
        /// </summary>
        public static Inventor.Application InventorApplication;

        /// <summary>
        /// Addin ID for customizing UI.
        /// </summary>
        public static string AddInCLSIDString;
    }

    internal static class ExtensionMethods
    {
        /// <summary>
        /// Get the associated drawing view object for the given dimension.
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static DrawingView GetView(this DrawingDimension dim)
        {
            if (dim is LinearGeneralDimension)
            {
                LinearGeneralDimension linearDim = (LinearGeneralDimension)dim;
                dynamic geom = (dynamic)linearDim.IntentOne.Geometry;
                return geom.Parent;
            }
            else if (dim is DiameterGeneralDimension)
            {
                DiameterGeneralDimension diameterDim = (DiameterGeneralDimension)dim;
                dynamic geom = (dynamic)diameterDim.Intent.Geometry;
                return geom.Parent;
            }
            else if (dim is RadiusGeneralDimension)
            {
                RadiusGeneralDimension radiusDim = (RadiusGeneralDimension)dim;
                dynamic geom = (dynamic)radiusDim.Intent.Geometry;
                return geom.Parent;
            }
            else if (dim is OrdinateDimension)
            {
                OrdinateDimension ordDim = (OrdinateDimension)dim;
                dynamic geom = (dynamic)ordDim.Intent.Geometry;
                return geom.Parent;
            }
            else if (dim is AngularGeneralDimension)
            {
                AngularGeneralDimension angularDim = (AngularGeneralDimension)dim;
                dynamic geom = (dynamic)angularDim.IntentOne.Geometry;
                return geom.Parent;
            }
            else
            {
                return null;
            }
        }


        public static Inventor.Property GetProperty(this Document doc, string propertyName)
        {
            foreach(PropertySet propSet in doc.PropertySets)
            {
                foreach(Property prop in propSet)
                {
                    if (prop.Name == propertyName) return prop;
                }
            }

            return null;
        }

        public static Inventor.Parameter GetParameter(this Document doc, string parameterName)
        {
            try
            {
                if (doc is PartDocument)
                {
                    PartDocument partDoc = (PartDocument)doc;
                    return partDoc.ComponentDefinition.Parameters[parameterName];
                }

                if (doc is AssemblyDocument)
                {
                    AssemblyDocument assemDoc = (AssemblyDocument)doc;
                    return assemDoc.ComponentDefinition.Parameters[parameterName];
                }
            }
            catch { }

            return null;
        }
    }

    /// <summary>
    /// Class for converting icons to IPictureDisp to be usable by Inventor API elements.
    /// </summary>
    public sealed class PictureDispConverter
    {
        [DllImport("OleAut32.dll", EntryPoint = "OleCreatePictureIndirect", ExactSpelling = true, PreserveSig = false)]

        private static extern stdole.IPictureDisp OleCreatePictureIndirect([MarshalAs(UnmanagedType.AsAny)] object picdesc, ref Guid iid, [MarshalAs(UnmanagedType.Bool)] bool fOwn);
        static Guid iPictureDispGuid = typeof(stdole.IPictureDisp).GUID;

        private static class PICTDESC
        {
            //Picture Types
            public const short PICTYPE_UNINITIALIZED = -1;
            public const short PICTYPE_NONE = 0;
            public const short PICTYPE_BITMAP = 1;
            public const short PICTYPE_METAFILE = 2;
            public const short PICTYPE_ICON = 3;
            public const short PICTYPE_ENHMETAFILE = 4;

            [StructLayout(LayoutKind.Sequential)]

            public class Icon
            {
                internal int cbSizeOfStruct = Marshal.SizeOf(typeof(PICTDESC.Icon));
                internal int picType = PICTDESC.PICTYPE_ICON;
                internal IntPtr hicon = IntPtr.Zero;
                internal int unused1;
                internal int unused2;
                internal Icon(System.Drawing.Icon icon)
                {
                    this.hicon = icon.ToBitmap().GetHicon();
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public class Bitmap
            {
                internal int cbSizeOfStruct = Marshal.SizeOf(typeof(PICTDESC.Bitmap));
                internal int picType = PICTDESC.PICTYPE_BITMAP;
                internal IntPtr hbitmap = IntPtr.Zero;
                internal IntPtr hpal = IntPtr.Zero;
                internal int unused;

                internal Bitmap(System.Drawing.Bitmap bitmap)
                {
                    this.hbitmap = bitmap.GetHbitmap();
                }

            }

        }

        public static stdole.IPictureDisp ToIPictureDisp(System.Drawing.Icon icon)
        {
            PICTDESC.Icon pictIcon = new PICTDESC.Icon(icon);
            return OleCreatePictureIndirect(pictIcon, ref iPictureDispGuid, true);
        }

        public static stdole.IPictureDisp ToIPictureDisp(System.Drawing.Bitmap bmp)
        {
            PICTDESC.Bitmap pictBmp = new PICTDESC.Bitmap(bmp);
            return OleCreatePictureIndirect(pictBmp, ref iPictureDispGuid, true);
        }
    }
}
