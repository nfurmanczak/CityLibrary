/*
 * www.gso-koeln.de 2020
 */
using CityLibrary.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CityLibrary.Svc
{
    public class SvcImport
    {
        public static IList<Member> ImportMembers1(string file) {
            using (var sr = new StreamReader(file, true)) {
                List<Member> members = new List<Member>();
                // process expected header
                sr.ReadLine();


                while (!sr.EndOfStream) {
                    break;
                }
                if (!sr.EndOfStream) {
                    sr.ReadLine();
                }


                if (!sr.EndOfStream) {
                    var hdr = sr.ReadLine();
                    if (hdr != "Gender,GivenName,Surname,StreetAddress,ZipCode,City,EmailAddress,Username,Password,Birthday") {
                        throw new ApplicationException("unexpected file header");
                    }
                }
                while (!sr.EndOfStream) {
                    var l = sr.ReadLine();
                    var a = l.Split(';', ',');
                    var p = new Member() {
                        FirstName = a[2].Trim(),
                        LastName = a[1].Trim(),
                        Birthday = DateTime.Parse(a[9].Trim(), CultureInfo.InvariantCulture)
                    };
                    if (p.LastName.StartsWith("S")) members.Add(p);
                }
                return members;
            }
        }

        public static IList<Member> ImportMembers2(string file) {
            using (var sr = new StreamReader(file, true)) {
                var members = sr.ReadToEnd()
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(l => l.Split(';', ','))
                .Select(a => new Member() {
                    FirstName = a[2].Trim(),
                    LastName = a[1].Trim(),
                    Birthday = DateTime.Parse(a[9].Trim(), CultureInfo.InvariantCulture)
                })
                .Where(p => p.LastName.StartsWith("A"))
                .ToList(); // ToArray() also possible
                return members;
            }
        }

        public static IList<Member> ImportMembers3(string file) {
            using (var sr = new StreamReader(file, true)) {
                List<Member> members = new List<Member>();

                // header mapping
                IDictionary<string, int> hdrs = null;

                // field mapping
                const string FirstName = "GivenName";
                const string LastName = "Surname";
                const string Birthday = "Birthday";

                // process expected header
                if (!sr.EndOfStream) {
                    var h = sr.ReadLine();
                    int i = 0;
                    hdrs = h.Split(';', ',').ToDictionary(s => s, s => i++);
                    if (!hdrs.ContainsKey(FirstName) ||
                        !hdrs.ContainsKey(LastName) ||
                        !hdrs.ContainsKey(Birthday)) {
                        throw new ApplicationException("unexpected file header");
                    }
                }
                while (!sr.EndOfStream) {
                    var l = sr.ReadLine();
                    var a = l.Split(';', ',');
                    var p = new Member() {
                        FirstName = a[hdrs[FirstName]].Trim(),
                        LastName = a[hdrs[LastName]].Trim(),
                        Birthday = DateTime.Parse(a[hdrs[Birthday]].Trim(), CultureInfo.InvariantCulture)
                    };
                    if (p.LastName.StartsWith("S")) members.Add(p);
                }
                return members;
            }
        }

        public static IList<Medium> ImportMediums1(string file) {
            using (var sr = new StreamReader(file, true)) {

                var media = new List<Medium>(); // result set

                // value cache variables
                string author = null;
                string category = null;
                string date = null;
                string ean = null;
                string format = null;
                string price = null;
                string publisher = null;
                string title = null;

                for (int nRow = 1; !sr.EndOfStream; nRow++) // count rows for diagnostics
                {
                    // read line tidy
                    var line = sr.ReadLine().Trim();
                    if (line.Length == 0) continue; // skip empty line
                    if (line.StartsWith("#")) continue; // skip comment line

                    // parse key-value-pair e.g. CATEGORY: Belletristik
                    var parts = line.Split(':');
                    var attrKey = parts[0].Trim();
                    var attrVal = parts[1].Trim();
                    if (attrVal.Length == 0) continue; // skip empty attributes

                    // sample each attribute into value cache
                    if (attrKey == "AUTHOR") author = attrVal;
                    if (attrKey == "CATEGORY") category = attrVal;
                    if (attrKey == "DATE") date = attrVal;
                    if (attrKey == "EAN") ean = attrVal;
                    if (attrKey == "FORMAT") format = attrVal;
                    if (attrKey == "PRICE") price = attrVal;
                    if (attrKey == "PUBLISHER") publisher = attrVal;
                    if (attrKey == "TITLE") title = attrVal;

                    if (attrKey == "PRICE") {
                        // parse medium
                        var m = new Medium(ean) {
                            Title = title,
                            Category = category,
                            Date = DateFromStr(date),
                            Kind = format,
                            Author = author,
                            Publisher = publisher,
                            Price = PriceFromStr(price),
                        };
                        media.Add(m);

                        // reset value cache to reduce potential data errors
                        author = null;
                        // category: keep as default
                        date = null;
                        ean = null;
                        // format: keep as default
                        price = null;
                        publisher = null;
                        title = null;
                    }
                }
                return media;
            }
        }

        public static IList<Medium> ImportMediums2(string file) {
            using (var sr = new StreamReader(file, true)) {

                var media = new List<Medium>(); // result set

                // attribute indices
                const int AUTHOR = 0;
                const int CATEGORY = 1;
                const int DATE = 2;
                const int EAN = 3;
                const int FORMAT = 4;
                const int PRICE = 5;
                const int PUBLISHER = 6;
                const int TITLE = 7;

                // attribute keys
                var attrKeys = new string[] {
                    "AUTHOR","CATEGORY","DATE","EAN",
                    "FORMAT","PRICE","PUBLISHER","TITLE"
                };

                // value cache
                var attrVals = new string[attrKeys.Length];

                for (int nRow = 1; !sr.EndOfStream; nRow++) // count rows for diagnostics
                {
                    // read line tidy
                    var line = sr.ReadLine().Trim();
                    if (line.Length == 0) continue; // skip empty line
                    if (line.StartsWith("#")) continue; // skip comment line

                    // parse key-value-pair e.g. CATEGORY: Belletristik
                    var parts = line.Split(':');
                    var attrKey = parts[0].Trim();
                    var attrVal = parts[1].Trim();
                    if (attrVal.Length == 0) continue; // skip empty attributes

                    // sample each attribute into value cache
                    int iKey = Array.IndexOf(attrKeys, attrKey);
                    attrVals[iKey] = attrVal;

                    if (attrKey == "PRICE") {
                        // parse medium
                        var m = new Medium(attrVals[EAN]) {
                            Title = attrVals[TITLE],
                            Category = attrVals[CATEGORY],
                            Date = DateFromStr(attrVals[DATE]),
                            Kind = attrVals[FORMAT],
                            Author = attrVals[AUTHOR],
                            Publisher = attrVals[PUBLISHER],
                            Price = PriceFromStr(attrVals[PRICE]),
                        };
                        media.Add(m);

                        // reset value cache to reduce potential data errors
                        attrVals = new string[attrKeys.Length];

                        // keep potential default values for next medium
                        attrVals[CATEGORY] = m.Category;
                        attrVals[FORMAT] = m.Kind;
                    }
                }
                return media;
            }
        }

        public enum MediumAttrKey { AUTHOR, CATEGORY, DATE, EAN, FORMAT, PRICE, PUBLISHER, TITLE };

        public static IList<Medium> ImportMediums3(string file) {
            using (var sr = new StreamReader(file, true)) {

                var media = new List<Medium>(); // result set

                // value cache
                var attrVals = new Dictionary<MediumAttrKey, string>();

                for (int nRow = 1; !sr.EndOfStream; nRow++) // count rows for diagnostics
                {
                    // read line tidy
                    var line = sr.ReadLine().Trim();
                    if (line.Length == 0) continue; // skip empty line
                    if (line.StartsWith("#")) continue; // skip comment line

                    // parse key-value-pair e.g. CATEGORY: Belletristik
                    var parts = line.Split(':');
                    var attrKey = parts[0].Trim();
                    var attrVal = parts[1].Trim();
                    if (attrVal.Length == 0) continue; // skip empty attributes

                    // sample each attribute into value cache
                    var key = (MediumAttrKey)Enum.Parse(typeof(MediumAttrKey), attrKey);
                    attrVals[key] = attrVal;

                    if (key == MediumAttrKey.PRICE) {
                        // parse medium
                        var m = new Medium(attrVals[MediumAttrKey.EAN]) {
                            Title = attrVals[MediumAttrKey.TITLE],
                            Category = attrVals[MediumAttrKey.CATEGORY],
                            Date = DateFromStr(Optional(attrVals, MediumAttrKey.DATE)),
                            Kind = attrVals[MediumAttrKey.FORMAT],
                            Author = Optional(attrVals, MediumAttrKey.AUTHOR),
                            Publisher = attrVals[MediumAttrKey.PUBLISHER],
                            Price = PriceFromStr(attrVals[MediumAttrKey.PRICE]),
                        };
                        media.Add(m);

                        // reset value cache to reduce potential data errors
                        attrVals = new Dictionary<MediumAttrKey, string>();

                        // keep potential default values for next medium
                        attrVals[MediumAttrKey.CATEGORY] = m.Category;
                        attrVals[MediumAttrKey.FORMAT] = m.Kind;
                    }
                }
                return media;
            }
        }

        public static DateTime? DateFromStr(string date) {
            return date == null ? (DateTime?)null : DateTime.ParseExact(date, "MMMM yyyy", null);
        }

        public static Decimal PriceFromStr(string price) {
            return Decimal.Parse(price.TrimEnd(' ', '€').Replace(',', '.'));
        }

        public static string Optional(Dictionary<MediumAttrKey, string> dict, MediumAttrKey key) {
            return dict.ContainsKey(key) ? dict[key] : null;
        }
    }
}
