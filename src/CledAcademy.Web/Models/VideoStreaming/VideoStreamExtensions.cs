using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace CledAcademy.Web.Models.VideoStreaming
{
    public static class VideoStreamExtensions
    {
        public static RangeHeaderValue GetRanges(this HttpContext context, long contentSize)
        {
            RangeHeaderValue rangesResult = null;

            string rangeHeader = context.Request.Headers["Range"];

            if (!string.IsNullOrEmpty(rangeHeader))
            {
                #region Range Description
                // rangeHeader contains the value of the Range HTTP Header and can have values like:
                //      Range: bytes=0-1            * Get bytes 0 and 1, inclusive
                //      Range: bytes=0-500          * Get bytes 0 to 500 (the first 501 bytes), inclusive
                //      Range: bytes=500-1000       * Get bytes 500 to 1000 (501 bytes in total), inclusive
                //      Range: bytes=-200           * Get the last 200 bytes
                //      Range: bytes=500-           * Get all bytes from byte 500 to the end
                #endregion

                // Remove "Ranges" and break up the ranges
                var range = rangeHeader.Replace("bytes=", string.Empty).Split(",".ToCharArray()).FirstOrDefault();

                rangesResult = new RangeHeaderValue();

                if(range!=null)
                { 
                    long startByte;
                    var currentRange = range.Split("-".ToCharArray());

                    var endByte = contentSize - 1;

                    if (long.TryParse(currentRange[0], out var parsedValue))
                        startByte = parsedValue;
                    else
                    {
                        // No beginning specified, get last n bytes of file
                        // We already parsed end, so subtract from total and
                        // make end the actual size of the file
                        startByte = contentSize - endByte;
                        endByte = contentSize - 1;
                    }
                    rangesResult.Ranges.Add(new RangeItemHeaderValue(startByte, endByte));
                }
            }
            return rangesResult;
        }
    }
}