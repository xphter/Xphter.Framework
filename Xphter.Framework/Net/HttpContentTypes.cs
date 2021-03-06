﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Provides corresponding HTTP Content-Type header values of some file types.
    /// </summary>
    internal static class HttpContentTypes {
        static HttpContentTypes() {
            #region Content Types Definition

            ContentTypes = new Dictionary<string, string[]> {
                { ".3dm", new string[]{ "x-world/x-3dmf" } },
                { ".3dmf", new string[]{ "x-world/x-3dmf" } },
                { ".a", new string[]{ "application/octet-stream" } },
                { ".aab", new string[]{ "application/x-authorware-bin" } },
                { ".aam", new string[]{ "application/x-authorware-map" } },
                { ".aas", new string[]{ "application/x-authorware-seg" } },
                { ".abc", new string[]{ "text/vnd.abc" } },
                { ".acgi", new string[]{ "text/html" } },
                { ".afl", new string[]{ "video/animaflex" } },
                { ".ai", new string[]{ "application/postscript" } },
                { ".aif", new string[]{ "audio/aiff", "audio/x-aiff" } },
                { ".aifc", new string[]{ "audio/aiff", "audio/x-aiff" } },
                { ".aiff", new string[]{ "audio/aiff", "audio/x-aiff" } },
                { ".aim", new string[]{ "application/x-aim" } },
                { ".aip", new string[]{ "text/x-audiosoft-intra" } },
                { ".ani", new string[]{ "application/x-navi-animation" } },
                { ".aos", new string[]{ "application/x-nokia-9000-communicator-add-on-software" } },
                { ".aps", new string[]{ "application/mime" } },
                { ".arc", new string[]{ "application/octet-stream" } },
                { ".arj", new string[]{ "application/arj", "application/octet-stream" } },
                { ".art", new string[]{ "image/x-jg" } },
                { ".asf", new string[]{ "video/x-ms-asf" } },
                { ".asm", new string[]{ "text/x-asm" } },
                { ".asp", new string[]{ "text/asp" } },
                { ".asx", new string[]{ "video/x-ms-asf", "video/x-ms-asf-plugin", "application/x-mplayer2" } },
                { ".au", new string[]{ "audio/x-au", "audio/basic" } },
                { ".avi", new string[]{ "video/avi", "video/msvideo", "video/x-msvideo", "application/x-troff-msvideo" } },
                { ".avs", new string[]{ "video/avs-video" } },
                { ".bcpio", new string[]{ "application/x-bcpio" } },
                { ".bin", new string[]{ "application/x-binary", "application/macbinary", "application/mac-binary", "application/x-macbinary", "application/octet-stream" } },
                { ".bm", new string[]{ "image/bmp" } },
                { ".bmp", new string[]{ "image/bmp", "image/x-windows-bmp" } },
                { ".boo", new string[]{ "application/book" } },
                { ".book", new string[]{ "application/book" } },
                { ".boz", new string[]{ "application/x-bzip2" } },
                { ".bsh", new string[]{ "application/x-bsh" } },
                { ".bz", new string[]{ "application/x-bzip" } },
                { ".bz2", new string[]{ "application/x-bzip2" } },
                { ".c", new string[]{ "text/x-c", "text/plain" } },
                { ".c++", new string[]{ "text/plain" } },
                { ".cat", new string[]{ "application/vnd.ms-pki.seccat" } },
                { ".cc", new string[]{ "text/x-c", "text/plain" } },
                { ".ccad", new string[]{ "application/clariscad" } },
                { ".cco", new string[]{ "application/x-cocoa" } },
                { ".cdf", new string[]{ "application/cdf", "application/x-cdf", "application/x-netcdf" } },
                { ".cer", new string[]{ "application/pkix-cert", "application/x-x509-ca-cert" } },
                { ".cha", new string[]{ "application/x-chat" } },
                { ".chat", new string[]{ "application/x-chat" } },
                { ".class", new string[]{ "application/java", "application/x-java-class", "application/java-byte-code" } },
                { ".com", new string[]{ "text/plain", "application/octet-stream" } },
                { ".conf", new string[]{ "text/plain" } },
                { ".cpio", new string[]{ "application/x-cpio" } },
                { ".cpp", new string[]{ "text/x-c" } },
                { ".cpt", new string[]{ "application/x-cpt", "application/x-compactpro", "application/mac-compactpro" } },
                { ".crl", new string[]{ "application/pkcs-crl", "application/pkix-crl" } },
                { ".crt", new string[]{ "application/pkix-cert", "application/x-x509-ca-cert", "application/x-x509-user-cert" } },
                { ".csh", new string[]{ "application/x-csh", "text/x-script.csh" } },
                { ".css", new string[]{ "text/css", "application/x-pointplus" } },
                { ".cxx", new string[]{ "text/plain" } },
                { ".dcr", new string[]{ "application/x-director" } },
                { ".deepv", new string[]{ "application/x-deepv" } },
                { ".def", new string[]{ "text/plain" } },
                { ".der", new string[]{ "application/x-x509-ca-cert" } },
                { ".dif", new string[]{ "video/x-dv" } },
                { ".dir", new string[]{ "application/x-director" } },
                { ".dl", new string[]{ "video/dl", "video/x-dl" } },
                { ".doc", new string[]{ "application/msword" } },
                { ".dot", new string[]{ "application/msword" } },
                { ".dp", new string[]{ "application/commonground" } },
                { ".drw", new string[]{ "application/drafting" } },
                { ".dump", new string[]{ "application/octet-stream" } },
                { ".dv", new string[]{ "video/x-dv" } },
                { ".dvi", new string[]{ "application/x-dvi" } },
                { ".dwf", new string[]{ "model/vnd.dwf", "drawing/x-dwf (old)" } },
                { ".dwg", new string[]{ "image/x-dwg", "image/vnd.dwg", "application/acad" } },
                { ".dxf", new string[]{ "image/x-dwg", "image/vnd.dwg", "application/dxf" } },
                { ".dxr", new string[]{ "application/x-director" } },
                { ".el", new string[]{ "text/x-script.elisp" } },
                { ".elc", new string[]{ "application/x-elc", "application/x-bytecode.elisp (compiled elisp)" } },
                { ".env", new string[]{ "application/x-envoy" } },
                { ".eps", new string[]{ "application/postscript" } },
                { ".es", new string[]{ "application/x-esrehber" } },
                { ".etx", new string[]{ "text/x-setext" } },
                { ".evy", new string[]{ "application/envoy", "application/x-envoy" } },
                { ".exe", new string[]{ "application/octet-stream" } },
                { ".f", new string[]{ "text/plain", "text/x-fortran" } },
                { ".f77", new string[]{ "text/x-fortran" } },
                { ".f90", new string[]{ "text/plain", "text/x-fortran" } },
                { ".fdf", new string[]{ "application/vnd.fdf" } },
                { ".fif", new string[]{ "image/fif", "application/fractals" } },
                { ".fli", new string[]{ "video/fli", "video/x-fli" } },
                { ".flo", new string[]{ "image/florian" } },
                { ".flx", new string[]{ "text/vnd.fmi.flexstor" } },
                { ".fmf", new string[]{ "video/x-atomic3d-feature" } },
                { ".for", new string[]{ "text/plain", "text/x-fortran" } },
                { ".fpx", new string[]{ "image/vnd.fpx", "image/vnd.net-fpx" } },
                { ".frl", new string[]{ "application/freeloader" } },
                { ".funk", new string[]{ "audio/make" } },
                { ".g", new string[]{ "text/plain" } },
                { ".g3", new string[]{ "image/g3fax" } },
                { ".gif", new string[]{ "image/gif" } },
                { ".gl", new string[]{ "video/gl", "video/x-gl" } },
                { ".gsd", new string[]{ "audio/x-gsm" } },
                { ".gsm", new string[]{ "audio/x-gsm" } },
                { ".gsp", new string[]{ "application/x-gsp" } },
                { ".gss", new string[]{ "application/x-gss" } },
                { ".gtar", new string[]{ "application/x-gtar" } },
                { ".gz", new string[]{ "application/x-gzip", "application/x-compressed" } },
                { ".gzip", new string[]{ "multipart/x-gzip", "application/x-gzip" } },
                { ".h", new string[]{ "text/x-h", "text/plain" } },
                { ".hdf", new string[]{ "application/x-hdf" } },
                { ".help", new string[]{ "application/x-helpfile" } },
                { ".hgl", new string[]{ "application/vnd.hp-hpgl" } },
                { ".hh", new string[]{ "text/x-h", "text/plain" } },
                { ".hlb", new string[]{ "text/x-script" } },
                { ".hlp", new string[]{ "application/hlp", "application/x-winhelp", "application/x-helpfile" } },
                { ".hpg", new string[]{ "application/vnd.hp-hpgl" } },
                { ".hpgl", new string[]{ "application/vnd.hp-hpgl" } },
                { ".hqx", new string[]{ "application/binhex", "application/binhex4", "application/mac-binhex", "application/x-binhex40", "application/mac-binhex40", "application/x-mac-binhex40" } },
                { ".hta", new string[]{ "application/hta" } },
                { ".htc", new string[]{ "text/x-component" } },
                { ".htm", new string[]{ "text/html" } },
                { ".html", new string[]{ "text/html" } },
                { ".htmls", new string[]{ "text/html" } },
                { ".htt", new string[]{ "text/webviewhtml" } },
                { ".htx", new string[]{ "text/html" } },
                { ".ice", new string[]{ "x-conference/x-cooltalk" } },
                { ".ico", new string[]{ "image/x-icon" } },
                { ".idc", new string[]{ "text/plain" } },
                { ".ief", new string[]{ "image/ief" } },
                { ".iefs", new string[]{ "image/ief" } },
                { ".iges", new string[]{ "model/iges", "application/iges" } },
                { ".igs", new string[]{ "model/iges", "application/iges" } },
                { ".ima", new string[]{ "application/x-ima" } },
                { ".imap", new string[]{ "application/x-httpd-imap" } },
                { ".inf", new string[]{ "application/inf" } },
                { ".ins", new string[]{ "application/x-internett-signup" } },
                { ".ip", new string[]{ "application/x-ip2" } },
                { ".isu", new string[]{ "video/x-isvideo" } },
                { ".it", new string[]{ "audio/it" } },
                { ".iv", new string[]{ "application/x-inventor" } },
                { ".ivr", new string[]{ "i-world/i-vrml" } },
                { ".ivy", new string[]{ "application/x-livescreen" } },
                { ".jam", new string[]{ "audio/x-jam" } },
                { ".jav", new string[]{ "text/plain", "text/x-java-source" } },
                { ".java", new string[]{ "text/plain", "text/x-java-source" } },
                { ".jcm", new string[]{ "application/x-java-commerce" } },
                { ".jfif", new string[]{ "image/jpeg", "image/pjpeg" } },
                { ".jfif-tbnl", new string[]{ "image/jpeg" } },
                { ".jpe", new string[]{ "image/jpeg", "image/pjpeg" } },
                { ".jpeg", new string[]{ "image/jpeg", "image/pjpeg" } },
                { ".jpg", new string[]{ "image/jpeg", "image/pjpeg" } },
                { ".jps", new string[]{ "image/x-jps" } },
                { ".js", new string[]{ "text/javascript", "text/ecmascript", "application/javascript", "application/ecmascript", "application/x-javascript" } },
                { ".jut", new string[]{ "image/jutvision" } },
                { ".kar", new string[]{ "audio/midi", "music/x-karaoke" } },
                { ".ksh", new string[]{ "application/x-ksh", "text/x-script.ksh" } },
                { ".la", new string[]{ "audio/nspaudio", "audio/x-nspaudio" } },
                { ".lam", new string[]{ "audio/x-liveaudio" } },
                { ".latex", new string[]{ "application/x-latex" } },
                { ".lha", new string[]{ "application/lha", "application/x-lha", "application/octet-stream" } },
                { ".lhx", new string[]{ "application/octet-stream" } },
                { ".list", new string[]{ "text/plain" } },
                { ".lma", new string[]{ "audio/nspaudio", "audio/x-nspaudio" } },
                { ".log", new string[]{ "text/plain" } },
                { ".lsp", new string[]{ "application/x-lisp", "text/x-script.lisp" } },
                { ".lst", new string[]{ "text/plain" } },
                { ".lsx", new string[]{ "text/x-la-asf" } },
                { ".ltx", new string[]{ "application/x-latex" } },
                { ".lzh", new string[]{ "application/x-lzh", "application/octet-stream" } },
                { ".lzx", new string[]{ "application/lzx", "application/x-lzx", "application/octet-stream" } },
                { ".m", new string[]{ "text/x-m", "text/plain" } },
                { ".m1v", new string[]{ "video/mpeg" } },
                { ".m2a", new string[]{ "audio/mpeg" } },
                { ".m2v", new string[]{ "video/mpeg" } },
                { ".m3u", new string[]{ "audio/x-mpequrl" } },
                { ".man", new string[]{ "application/x-troff-man" } },
                { ".map", new string[]{ "application/x-navimap" } },
                { ".mar", new string[]{ "text/plain" } },
                { ".mbd", new string[]{ "application/mbedlet" } },
                { ".mc$", new string[]{ "application/x-magic-cap-package-1.0" } },
                { ".mcd", new string[]{ "application/mcad", "application/x-mathcad" } },
                { ".mcf", new string[]{ "text/mcf", "image/vasa" } },
                { ".mcp", new string[]{ "application/netmc" } },
                { ".me", new string[]{ "application/x-troff-me" } },
                { ".mht", new string[]{ "message/rfc822" } },
                { ".mhtml", new string[]{ "message/rfc822" } },
                { ".mid", new string[]{ "audio/midi", "audio/x-mid", "audio/x-midi", "x-music/x-midi", "music/crescendo", "application/x-midi" } },
                { ".midi", new string[]{ "audio/midi", "audio/x-mid", "audio/x-midi", "x-music/x-midi", "music/crescendo", "application/x-midi" } },
                { ".mif", new string[]{ "application/x-mif", "application/x-frame" } },
                { ".mime", new string[]{ "www/mime", "message/rfc822" } },
                { ".mjf", new string[]{ "audio/x-vnd.audioexplosion.mjuicemediafile" } },
                { ".mjpg", new string[]{ "video/x-motion-jpeg" } },
                { ".mm", new string[]{ "application/base64", "application/x-meme" } },
                { ".mme", new string[]{ "application/base64" } },
                { ".mod", new string[]{ "audio/mod", "audio/x-mod" } },
                { ".moov", new string[]{ "video/quicktime" } },
                { ".mov", new string[]{ "video/quicktime" } },
                { ".movie", new string[]{ "video/x-sgi-movie" } },
                { ".mp2", new string[]{ "audio/mpeg", "video/mpeg", "audio/x-mpeg", "video/x-mpeg", "video/x-mpeq2a" } },
                { ".mp3", new string[]{ "video/mpeg", "audio/mpeg3", "video/x-mpeg", "audio/x-mpeg-3" } },
                { ".mpa", new string[]{ "audio/mpeg", "video/mpeg" } },
                { ".mpc", new string[]{ "application/x-project" } },
                { ".mpe", new string[]{ "video/mpeg" } },
                { ".mpeg", new string[]{ "video/mpeg" } },
                { ".mpg", new string[]{ "audio/mpeg", "video/mpeg" } },
                { ".mpga", new string[]{ "audio/mpeg" } },
                { ".mpp", new string[]{ "application/vnd.ms-project" } },
                { ".mpt", new string[]{ "application/x-project" } },
                { ".mpv", new string[]{ "application/x-project" } },
                { ".mpx", new string[]{ "application/x-project" } },
                { ".mrc", new string[]{ "application/marc" } },
                { ".ms", new string[]{ "application/x-troff-ms" } },
                { ".mv", new string[]{ "video/x-sgi-movie" } },
                { ".my", new string[]{ "audio/make" } },
                { ".mzz", new string[]{ "application/x-vnd.audioexplosion.mzz" } },
                { ".nap", new string[]{ "image/naplps" } },
                { ".naplps", new string[]{ "image/naplps" } },
                { ".nc", new string[]{ "application/x-netcdf" } },
                { ".ncm", new string[]{ "application/vnd.nokia.configuration-message" } },
                { ".nif", new string[]{ "image/x-niff" } },
                { ".niff", new string[]{ "image/x-niff" } },
                { ".nix", new string[]{ "application/x-mix-transfer" } },
                { ".nsc", new string[]{ "application/x-conference" } },
                { ".nvd", new string[]{ "application/x-navidoc" } },
                { ".o", new string[]{ "application/octet-stream" } },
                { ".oda", new string[]{ "application/oda" } },
                { ".omc", new string[]{ "application/x-omc" } },
                { ".omcd", new string[]{ "application/x-omcdatamaker" } },
                { ".omcr", new string[]{ "application/x-omcregerator" } },
                { ".p", new string[]{ "text/x-pascal" } },
                { ".p10", new string[]{ "application/pkcs10", "application/x-pkcs10" } },
                { ".p12", new string[]{ "application/pkcs-12", "application/x-pkcs12" } },
                { ".p7a", new string[]{ "application/x-pkcs7-signature" } },
                { ".p7c", new string[]{ "application/pkcs7-mime", "application/x-pkcs7-mime" } },
                { ".p7m", new string[]{ "application/pkcs7-mime", "application/x-pkcs7-mime" } },
                { ".p7r", new string[]{ "application/x-pkcs7-certreqresp" } },
                { ".p7s", new string[]{ "application/pkcs7-signature" } },
                { ".part", new string[]{ "application/pro_eng" } },
                { ".pas", new string[]{ "text/pascal" } },
                { ".pbm", new string[]{ "image/x-portable-bitmap" } },
                { ".pcl", new string[]{ "application/x-pcl", "application/vnd.hp-pcl" } },
                { ".pct", new string[]{ "image/x-pict" } },
                { ".pcx", new string[]{ "image/x-pcx" } },
                { ".pdb", new string[]{ "chemical/x-pdb" } },
                { ".pdf", new string[]{ "application/pdf" } },
                { ".pfunk", new string[]{ "audio/make", "audio/make.my.funk" } },
                { ".pgm", new string[]{ "image/x-portable-graymap", "image/x-portable-greymap" } },
                { ".pic", new string[]{ "image/pict" } },
                { ".pict", new string[]{ "image/pict" } },
                { ".pkg", new string[]{ "application/x-newton-compatible-pkg" } },
                { ".pko", new string[]{ "application/vnd.ms-pki.pko" } },
                { ".pl", new string[]{ "text/plain", "text/x-script.perl" } },
                { ".plx", new string[]{ "application/x-pixclscript" } },
                { ".pm", new string[]{ "image/x-xpixmap", "text/x-script.perl-module" } },
                { ".pm4", new string[]{ "application/x-pagemaker" } },
                { ".pm5", new string[]{ "application/x-pagemaker" } },
                { ".png", new string[]{ "image/png" } },
                { ".pnm", new string[]{ "image/x-portable-anymap", "application/x-portable-anymap" } },
                { ".pot", new string[]{ "application/mspowerpoint", "application/vnd.ms-powerpoint" } },
                { ".pov", new string[]{ "model/x-pov" } },
                { ".ppa", new string[]{ "application/vnd.ms-powerpoint" } },
                { ".ppm", new string[]{ "image/x-portable-pixmap" } },
                { ".pps", new string[]{ "application/mspowerpoint", "application/vnd.ms-powerpoint" } },
                { ".ppt", new string[]{ "application/powerpoint", "application/mspowerpoint", "application/x-mspowerpoint", "application/vnd.ms-powerpoint" } },
                { ".ppz", new string[]{ "application/mspowerpoint" } },
                { ".pre", new string[]{ "application/x-freelance" } },
                { ".prt", new string[]{ "application/pro_eng" } },
                { ".ps", new string[]{ "application/postscript" } },
                { ".psd", new string[]{ "application/octet-stream" } },
                { ".pvu", new string[]{ "paleovu/x-pv" } },
                { ".pwz", new string[]{ "application/vnd.ms-powerpoint" } },
                { ".py", new string[]{ "text/x-script.phyton" } },
                { ".pyc", new string[]{ "applicaiton/x-bytecode.python" } },
                { ".qcp", new string[]{ "audio/vnd.qcelp" } },
                { ".qd3", new string[]{ "x-world/x-3dmf" } },
                { ".qd3d", new string[]{ "x-world/x-3dmf" } },
                { ".qif", new string[]{ "image/x-quicktime" } },
                { ".qt", new string[]{ "video/quicktime" } },
                { ".qtc", new string[]{ "video/x-qtc" } },
                { ".qti", new string[]{ "image/x-quicktime" } },
                { ".qtif", new string[]{ "image/x-quicktime" } },
                { ".ra", new string[]{ "audio/x-realaudio", "audio/x-pn-realaudio", "audio/x-pn-realaudio-plugin" } },
                { ".ram", new string[]{ "audio/x-pn-realaudio" } },
                { ".ras", new string[]{ "image/cmu-raster", "image/x-cmu-raster", "application/x-cmu-raster" } },
                { ".rast", new string[]{ "image/cmu-raster" } },
                { ".rexx", new string[]{ "text/x-script.rexx" } },
                { ".rf", new string[]{ "image/vnd.rn-realflash" } },
                { ".rgb", new string[]{ "image/x-rgb" } },
                { ".rm", new string[]{ "audio/x-pn-realaudio", "application/vnd.rn-realmedia" } },
                { ".rmi", new string[]{ "audio/mid" } },
                { ".rmm", new string[]{ "audio/x-pn-realaudio" } },
                { ".rmp", new string[]{ "audio/x-pn-realaudio", "audio/x-pn-realaudio-plugin" } },
                { ".rng", new string[]{ "application/ringing-tones", "application/vnd.nokia.ringing-tone" } },
                { ".rnx", new string[]{ "application/vnd.rn-realplayer" } },
                { ".roff", new string[]{ "application/x-troff" } },
                { ".rp", new string[]{ "image/vnd.rn-realpix" } },
                { ".rpm", new string[]{ "audio/x-pn-realaudio-plugin" } },
                { ".rt", new string[]{ "text/richtext", "text/vnd.rn-realtext" } },
                { ".rtf", new string[]{ "text/richtext", "application/rtf", "application/x-rtf" } },
                { ".rtx", new string[]{ "text/richtext", "application/rtf" } },
                { ".rv", new string[]{ "video/vnd.rn-realvideo" } },
                { ".s", new string[]{ "text/x-asm" } },
                { ".s3m", new string[]{ "audio/s3m" } },
                { ".saveme", new string[]{ "application/octet-stream" } },
                { ".sbk", new string[]{ "application/x-tbook" } },
                { ".scm", new string[]{ "video/x-scm", "text/x-script.guile", "text/x-script.scheme", "application/x-lotusscreencam" } },
                { ".sdml", new string[]{ "text/plain" } },
                { ".sdp", new string[]{ "application/sdp", "application/x-sdp" } },
                { ".sdr", new string[]{ "application/sounder" } },
                { ".sea", new string[]{ "application/sea", "application/x-sea" } },
                { ".set", new string[]{ "application/set" } },
                { ".sgm", new string[]{ "text/sgml", "text/x-sgml" } },
                { ".sgml", new string[]{ "text/sgml", "text/x-sgml" } },
                { ".sh", new string[]{ "application/x-sh", "text/x-script.sh", "application/x-bsh", "application/x-shar" } },
                { ".shar", new string[]{ "application/x-bsh", "application/x-shar" } },
                { ".shtml", new string[]{ "text/html", "text/x-server-parsed-html" } },
                { ".sid", new string[]{ "audio/x-psid" } },
                { ".sit", new string[]{ "application/x-sit", "application/x-stuffit" } },
                { ".skd", new string[]{ "application/x-koan" } },
                { ".skm", new string[]{ "application/x-koan" } },
                { ".skp", new string[]{ "application/x-koan" } },
                { ".skt", new string[]{ "application/x-koan" } },
                { ".sl", new string[]{ "application/x-seelogo" } },
                { ".smi", new string[]{ "application/smil" } },
                { ".smil", new string[]{ "application/smil" } },
                { ".snd", new string[]{ "audio/basic", "audio/x-adpcm" } },
                { ".sol", new string[]{ "application/solids" } },
                { ".spc", new string[]{ "text/x-speech", "application/x-pkcs7-certificates" } },
                { ".spl", new string[]{ "application/futuresplash" } },
                { ".spr", new string[]{ "application/x-sprite" } },
                { ".sprite", new string[]{ "application/x-sprite" } },
                { ".src", new string[]{ "application/x-wais-source" } },
                { ".ssi", new string[]{ "text/x-server-parsed-html" } },
                { ".ssm", new string[]{ "application/streamingmedia" } },
                { ".sst", new string[]{ "application/vnd.ms-pki.certstore" } },
                { ".step", new string[]{ "application/step" } },
                { ".stl", new string[]{ "application/sla", "application/x-navistyle", "application/vnd.ms-pki.stl" } },
                { ".stp", new string[]{ "application/step" } },
                { ".sv4cpio", new string[]{ "application/x-sv4cpio" } },
                { ".sv4crc", new string[]{ "application/x-sv4crc" } },
                { ".svf", new string[]{ "image/x-dwg", "image/vnd.dwg" } },
                { ".svr", new string[]{ "x-world/x-svr", "application/x-world" } },
                { ".swf", new string[]{ "application/x-shockwave-flash" } },
                { ".t", new string[]{ "application/x-troff" } },
                { ".talk", new string[]{ "text/x-speech" } },
                { ".tar", new string[]{ "application/x-tar" } },
                { ".tbk", new string[]{ "application/x-tbook", "application/toolbook" } },
                { ".tcl", new string[]{ "application/x-tcl", "text/x-script.tcl" } },
                { ".tcsh", new string[]{ "text/x-script.tcsh" } },
                { ".tex", new string[]{ "application/x-tex" } },
                { ".texi", new string[]{ "application/x-texinfo" } },
                { ".texinfo", new string[]{ "application/x-texinfo" } },
                { ".text", new string[]{ "text/plain", "application/plain" } },
                { ".tgz", new string[]{ "application/gnutar", "application/x-compressed" } },
                { ".tif", new string[]{ "image/tiff", "image/x-tiff" } },
                { ".tiff", new string[]{ "image/tiff", "image/x-tiff" } },
                { ".tr", new string[]{ "application/x-troff" } },
                { ".tsi", new string[]{ "audio/tsp-audio" } },
                { ".tsp", new string[]{ "audio/tsplayer", "application/dsptype" } },
                { ".tsv", new string[]{ "text/tab-separated-values" } },
                { ".turbot", new string[]{ "image/florian" } },
                { ".txt", new string[]{ "text/plain" } },
                { ".uil", new string[]{ "text/x-uil" } },
                { ".uni", new string[]{ "text/uri-list" } },
                { ".unis", new string[]{ "text/uri-list" } },
                { ".unv", new string[]{ "application/i-deas" } },
                { ".uri", new string[]{ "text/uri-list" } },
                { ".uris", new string[]{ "text/uri-list" } },
                { ".ustar", new string[]{ "multipart/x-ustar", "application/x-ustar" } },
                { ".uu", new string[]{ "text/x-uuencode", "application/octet-stream" } },
                { ".uue", new string[]{ "text/x-uuencode" } },
                { ".vcd", new string[]{ "application/x-cdlink" } },
                { ".vcs", new string[]{ "text/x-vcalendar" } },
                { ".vda", new string[]{ "application/vda" } },
                { ".vdo", new string[]{ "video/vdo" } },
                { ".vew", new string[]{ "application/groupwise" } },
                { ".viv", new string[]{ "video/vivo", "video/vnd.vivo" } },
                { ".vivo", new string[]{ "video/vivo", "video/vnd.vivo" } },
                { ".vmd", new string[]{ "application/vocaltec-media-desc" } },
                { ".vmf", new string[]{ "application/vocaltec-media-file" } },
                { ".voc", new string[]{ "audio/voc", "audio/x-voc" } },
                { ".vos", new string[]{ "video/vosaic" } },
                { ".vox", new string[]{ "audio/voxware" } },
                { ".vqe", new string[]{ "audio/x-twinvq-plugin" } },
                { ".vqf", new string[]{ "audio/x-twinvq" } },
                { ".vql", new string[]{ "audio/x-twinvq-plugin" } },
                { ".vrml", new string[]{ "model/vrml", "x-world/x-vrml", "application/x-vrml" } },
                { ".vrt", new string[]{ "x-world/x-vrt" } },
                { ".vsd", new string[]{ "application/x-visio" } },
                { ".vst", new string[]{ "application/x-visio" } },
                { ".vsw", new string[]{ "application/x-visio" } },
                { ".w60", new string[]{ "application/wordperfect6.0" } },
                { ".w61", new string[]{ "application/wordperfect6.1" } },
                { ".w6w", new string[]{ "application/msword" } },
                { ".wav", new string[]{ "audio/wav", "audio/x-wav" } },
                { ".wb1", new string[]{ "application/x-qpro" } },
                { ".wbmp", new string[]{ "image/vnd.wap.wbmp" } },
                { ".web", new string[]{ "application/vnd.xara" } },
                { ".wiz", new string[]{ "application/msword" } },
                { ".wk1", new string[]{ "application/x-123" } },
                { ".wmf", new string[]{ "windows/metafile" } },
                { ".wml", new string[]{ "text/vnd.wap.wml" } },
                { ".wmlc", new string[]{ "application/vnd.wap.wmlc" } },
                { ".wmls", new string[]{ "text/vnd.wap.wmlscript" } },
                { ".wmlsc", new string[]{ "application/vnd.wap.wmlscriptc" } },
                { ".word", new string[]{ "application/msword" } },
                { ".wp", new string[]{ "application/wordperfect" } },
                { ".wp5", new string[]{ "application/wordperfect", "application/wordperfect6.0" } },
                { ".wp6", new string[]{ "application/wordperfect" } },
                { ".wpd", new string[]{ "application/x-wpwin", "application/wordperfect" } },
                { ".wq1", new string[]{ "application/x-lotus" } },
                { ".wri", new string[]{ "application/x-wri", "application/mswrite" } },
                { ".wrl", new string[]{ "model/vrml", "x-world/x-vrml", "application/x-world" } },
                { ".wrz", new string[]{ "model/vrml", "x-world/x-vrml" } },
                { ".wsc", new string[]{ "text/scriplet" } },
                { ".wsrc", new string[]{ "application/x-wais-source" } },
                { ".wtk", new string[]{ "application/x-wintalk" } },
                { ".xbm", new string[]{ "image/xbm", "image/x-xbm", "image/x-xbitmap" } },
                { ".xdr", new string[]{ "video/x-amt-demorun" } },
                { ".xgz", new string[]{ "xgl/drawing" } },
                { ".xif", new string[]{ "image/vnd.xiff" } },
                { ".xl", new string[]{ "application/excel" } },
                { ".xla", new string[]{ "application/excel", "application/x-excel", "application/x-msexcel" } },
                { ".xlb", new string[]{ "application/excel", "application/x-excel", "application/vnd.ms-excel" } },
                { ".xlc", new string[]{ "application/excel", "application/x-excel", "application/vnd.ms-excel" } },
                { ".xld", new string[]{ "application/excel", "application/x-excel" } },
                { ".xlk", new string[]{ "application/excel", "application/x-excel" } },
                { ".xll", new string[]{ "application/excel", "application/x-excel", "application/vnd.ms-excel" } },
                { ".xlm", new string[]{ "application/excel", "application/x-excel", "application/vnd.ms-excel" } },
                { ".xls", new string[]{ "application/excel", "application/x-excel", "application/x-msexcel", "application/vnd.ms-excel" } },
                { ".xlt", new string[]{ "application/excel", "application/x-excel" } },
                { ".xlv", new string[]{ "application/excel", "application/x-excel" } },
                { ".xlw", new string[]{ "application/excel", "application/x-excel", "application/x-msexcel", "application/vnd.ms-excel" } },
                { ".xm", new string[]{ "audio/xm" } },
                { ".xml", new string[]{ "text/xml", "application/xml" } },
                { ".xmz", new string[]{ "xgl/movie" } },
                { ".xpix", new string[]{ "application/x-vnd.ls-xpix" } },
                { ".xpm", new string[]{ "image/xpm", "image/x-xpixmap" } },
                { ".x-png", new string[]{ "image/png" } },
                { ".xsr", new string[]{ "video/x-amt-showrun" } },
                { ".xwd", new string[]{ "image/x-xwd", "image/x-xwindowdump" } },
                { ".xyz", new string[]{ "chemical/x-pdb" } },
                { ".z", new string[]{ "application/x-compress", "application/x-compressed" } },
                { ".zip", new string[]{ "application/zip", "multipart/x-zip", "application/x-compressed", "application/x-zip-compressed" } },
                { ".zoo", new string[]{ "application/octet-stream" } },
                { ".zsh", new string[]{ "text/x-script.zsh" } },
            }.AsReadOnly();

            #endregion
        }

        /// <summary>
        /// The default HTTP Content-Type header value of unknown file types.
        /// </summary>
        public static string DEFAULT_CONTENT_TYPE = "application/octet-stream";

        /// <summary>
        /// Gets a read-only dictionary of HTTP Content-Type header values, the Key is file extension.
        /// </summary>
        public static readonly IDictionary<string, string[]> ContentTypes;
    }
}
