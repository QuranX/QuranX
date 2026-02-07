using System.Collections.Generic;
using System.Linq;

namespace QuranX.Shared.Models;

public static class QuranStructure
{
    static Dictionary<int, Chapter> _Chapters;

    static QuranStructure()
    {
        _Chapters = new Dictionary<int, Chapter>();
        GenerateData();
    }

    public static Chapter Chapter(int chapterNumber) => _Chapters[chapterNumber];
    public static IEnumerable<Chapter> Chapters => _Chapters.Values.OrderBy(x => x.ChapterNumber);

    public static bool TryValidateChapterAndVerse(int chapterNumber, int verseNumber)
    {
        if (chapterNumber < 1 || chapterNumber > 114)
            return false;
        if (verseNumber < 1 || verseNumber > Chapter(chapterNumber).NumberOfVerses)
            return false;
        return true;
    }

    private static void GenerateData()
    {
        _Chapters[1] = new Chapter(chapterNumber: 1, arabicName: "الفاتحة", englishName: "Al Fatiha (The opening)", period: "Makka", numberOfVerses: 7, revelationOrder: 5);
        _Chapters[2] = new Chapter(chapterNumber: 2, arabicName: "البقرة", englishName: "Al Baqara (The cow)", period: "Madina", numberOfVerses: 286, revelationOrder: 87);
        _Chapters[3] = new Chapter(chapterNumber: 3, arabicName: "آل عمران", englishName: "Al Imran (The family of Imran)", period: "Madina", numberOfVerses: 200, revelationOrder: 89);
        _Chapters[4] = new Chapter(chapterNumber: 4, arabicName: "النساء", englishName: "An Nisa (The women)", period: "Madina", numberOfVerses: 176, revelationOrder: 92);
        _Chapters[5] = new Chapter(chapterNumber: 5, arabicName: "المائدة", englishName: "Al Maida (The food)", period: "Madina", numberOfVerses: 120, revelationOrder: 112);
        _Chapters[6] = new Chapter(chapterNumber: 6, arabicName: "الأنعام", englishName: "Al Anam (Cattle)", period: "Makka", numberOfVerses: 165, revelationOrder: 55);
        _Chapters[7] = new Chapter(chapterNumber: 7, arabicName: "الأعراف", englishName: "Al Araf (The heights)", period: "Makka", numberOfVerses: 206, revelationOrder: 39);
        _Chapters[8] = new Chapter(chapterNumber: 8, arabicName: "الأنفال", englishName: "Al Anfal (Spoils of war)", period: "Madina", numberOfVerses: 75, revelationOrder: 88);
        _Chapters[9] = new Chapter(chapterNumber: 9, arabicName: "التوبة", englishName: "Al Tawba (Immunity)", period: "Madina", numberOfVerses: 129, revelationOrder: 113);
        _Chapters[10] = new Chapter(chapterNumber: 10, arabicName: "يونس", englishName: "Yunus (Jonah)", period: "Makka", numberOfVerses: 109, revelationOrder: 51);
        _Chapters[11] = new Chapter(chapterNumber: 11, arabicName: "هود", englishName: "Hud", period: "Makka", numberOfVerses: 123, revelationOrder: 52);
        _Chapters[12] = new Chapter(chapterNumber: 12, arabicName: "يوسف", englishName: "Yusuf (Joseph)", period: "Makka", numberOfVerses: 111, revelationOrder: 53);
        _Chapters[13] = new Chapter(chapterNumber: 13, arabicName: "الرعد", englishName: "Ar Rad (The thunder)", period: "Makka", numberOfVerses: 43, revelationOrder: 96);
        _Chapters[14] = new Chapter(chapterNumber: 14, arabicName: "ابراهيم", englishName: "Ibrahim (Abraham)", period: "Makka", numberOfVerses: 52, revelationOrder: 72);
        _Chapters[15] = new Chapter(chapterNumber: 15, arabicName: "الحجر", englishName: "Al Hijr (The rock)", period: "Makka", numberOfVerses: 99, revelationOrder: 54);
        _Chapters[16] = new Chapter(chapterNumber: 16, arabicName: "النحل", englishName: "An Nahl (The bee)", period: "Makka", numberOfVerses: 128, revelationOrder: 70);
        _Chapters[17] = new Chapter(chapterNumber: 17, arabicName: "الإسراء", englishName: "Bani Israil (The Israelites)", period: "Makka", numberOfVerses: 111, revelationOrder: 50);
        _Chapters[18] = new Chapter(chapterNumber: 18, arabicName: "الكهف", englishName: "Al Kahf (The cave)", period: "Makka", numberOfVerses: 110, revelationOrder: 69);
        _Chapters[19] = new Chapter(chapterNumber: 19, arabicName: "مريم", englishName: "Maryamn (Mary)", period: "Makka", numberOfVerses: 98, revelationOrder: 44);
        _Chapters[20] = new Chapter(chapterNumber: 20, arabicName: "طه", englishName: "Ta-Ha", period: "Makka", numberOfVerses: 135, revelationOrder: 45);
        _Chapters[21] = new Chapter(chapterNumber: 21, arabicName: "الأنبياء", englishName: "Al Anbiya (The prophets)", period: "Makka", numberOfVerses: 112, revelationOrder: 73);
        _Chapters[22] = new Chapter(chapterNumber: 22, arabicName: "الحج", englishName: "Al Hajj (The pilgrimage)", period: "Madina", numberOfVerses: 78, revelationOrder: 103);
        _Chapters[23] = new Chapter(chapterNumber: 23, arabicName: "المؤمنون", englishName: "Al Muminun (The believers)", period: "Makka", numberOfVerses: 118, revelationOrder: 74);
        _Chapters[24] = new Chapter(chapterNumber: 24, arabicName: "النور", englishName: "An Nur (The light)", period: "Madina", numberOfVerses: 64, revelationOrder: 102);
        _Chapters[25] = new Chapter(chapterNumber: 25, arabicName: "الفرقان", englishName: "Al Furqan (The discrimination)", period: "Makka", numberOfVerses: 77, revelationOrder: 42);
        _Chapters[26] = new Chapter(chapterNumber: 26, arabicName: "الشعراء", englishName: "Ash Shuara (The poets)", period: "Makka", numberOfVerses: 227, revelationOrder: 47);
        _Chapters[27] = new Chapter(chapterNumber: 27, arabicName: "النمل", englishName: "An Naml (The ant)", period: "Makka", numberOfVerses: 93, revelationOrder: 48);
        _Chapters[28] = new Chapter(chapterNumber: 28, arabicName: "القصص", englishName: "Al Qasas (The narrative)", period: "Makka", numberOfVerses: 88, revelationOrder: 49);
        _Chapters[29] = new Chapter(chapterNumber: 29, arabicName: "العنكبوت", englishName: "Al Ankabut (The spider)", period: "Makka", numberOfVerses: 69, revelationOrder: 85);
        _Chapters[30] = new Chapter(chapterNumber: 30, arabicName: "الروم", englishName: "Ar Rum (The Romans)", period: "Makka", numberOfVerses: 60, revelationOrder: 84);
        _Chapters[31] = new Chapter(chapterNumber: 31, arabicName: "لقمان", englishName: "Luqman", period: "Makka", numberOfVerses: 34, revelationOrder: 57);
        _Chapters[32] = new Chapter(chapterNumber: 32, arabicName: "السجدة", englishName: "As Sajda (The adoration)", period: "Makka", numberOfVerses: 30, revelationOrder: 75);
        _Chapters[33] = new Chapter(chapterNumber: 33, arabicName: "الأحزاب", englishName: "Al Ahzab (The allies)", period: "Madina", numberOfVerses: 73, revelationOrder: 90);
        _Chapters[34] = new Chapter(chapterNumber: 34, arabicName: "سبإ", englishName: "Al Saba (The Saba)", period: "Makka", numberOfVerses: 54, revelationOrder: 58);
        _Chapters[35] = new Chapter(chapterNumber: 35, arabicName: "فاطر", englishName: "Al Fatir (The angels)", period: "Makka", numberOfVerses: 45, revelationOrder: 43);
        _Chapters[36] = new Chapter(chapterNumber: 36, arabicName: "يس", englishName: "Ya Sin", period: "Makka", numberOfVerses: 83, revelationOrder: 41);
        _Chapters[37] = new Chapter(chapterNumber: 37, arabicName: "الصافات", englishName: "As Saffat (Those who set up ranks)", period: "Makka", numberOfVerses: 182, revelationOrder: 56);
        _Chapters[38] = new Chapter(chapterNumber: 38, arabicName: "ص", englishName: "Saad", period: "Makka", numberOfVerses: 88, revelationOrder: 38);
        _Chapters[39] = new Chapter(chapterNumber: 39, arabicName: "الزمر", englishName: "Az Zumar (The troops)", period: "Makka", numberOfVerses: 75, revelationOrder: 59);
        _Chapters[40] = new Chapter(chapterNumber: 40, arabicName: "غافر", englishName: "Al Ghafir (The forgiver)", period: "Makka", numberOfVerses: 85, revelationOrder: 60);
        _Chapters[41] = new Chapter(chapterNumber: 41, arabicName: "فصلت", englishName: "Fussilat (Explained in detail)", period: "Makka", numberOfVerses: 54, revelationOrder: 61);
        _Chapters[42] = new Chapter(chapterNumber: 42, arabicName: "الشورى", englishName: "Ash Shura (The council)", period: "Makka", numberOfVerses: 53, revelationOrder: 62);
        _Chapters[43] = new Chapter(chapterNumber: 43, arabicName: "الزخرف", englishName: "Az Zukhruf (Gold)", period: "Makka", numberOfVerses: 89, revelationOrder: 63);
        _Chapters[44] = new Chapter(chapterNumber: 44, arabicName: "الدخان", englishName: "Ad Dukhan (Drought)", period: "Makka", numberOfVerses: 59, revelationOrder: 64);
        _Chapters[45] = new Chapter(chapterNumber: 45, arabicName: "الجاثية", englishName: "Al Jathiya (Kneeling)", period: "Makka", numberOfVerses: 37, revelationOrder: 65);
        _Chapters[46] = new Chapter(chapterNumber: 46, arabicName: "الأحقاف", englishName: "Al Ahqaf (The dunes)", period: "Makka", numberOfVerses: 35, revelationOrder: 66);
        _Chapters[47] = new Chapter(chapterNumber: 47, arabicName: "محمد", englishName: "Muhammad", period: "Madina", numberOfVerses: 38, revelationOrder: 95);
        _Chapters[48] = new Chapter(chapterNumber: 48, arabicName: "الفتح", englishName: "Al Fath (Victory)", period: "Madina", numberOfVerses: 29, revelationOrder: 111);
        _Chapters[49] = new Chapter(chapterNumber: 49, arabicName: "الحجرات", englishName: "Al Hujraat (The apartments)", period: "Madina", numberOfVerses: 18, revelationOrder: 106);
        _Chapters[50] = new Chapter(chapterNumber: 50, arabicName: "ق", englishName: "Qaf", period: "Makka", numberOfVerses: 45, revelationOrder: 34);
        _Chapters[51] = new Chapter(chapterNumber: 51, arabicName: "الذاريات", englishName: "Adh Dhariyat (The scatterers)", period: "Makka", numberOfVerses: 60, revelationOrder: 67);
        _Chapters[52] = new Chapter(chapterNumber: 52, arabicName: "الطور", englishName: "At Tur (The mount)", period: "Makka", numberOfVerses: 49, revelationOrder: 76);
        _Chapters[53] = new Chapter(chapterNumber: 53, arabicName: "النجم", englishName: "An Najm (The star)", period: "Makka", numberOfVerses: 62, revelationOrder: 23);
        _Chapters[54] = new Chapter(chapterNumber: 54, arabicName: "القمر", englishName: "Al Qamar (The Moon)", period: "Makka", numberOfVerses: 55, revelationOrder: 37);
        _Chapters[55] = new Chapter(chapterNumber: 55, arabicName: "الرحمن", englishName: "Ar Rahman (The beneficent)", period: "Makka", numberOfVerses: 78, revelationOrder: 97);
        _Chapters[56] = new Chapter(chapterNumber: 56, arabicName: "الواقعة", englishName: "Al Waqia (The event)", period: "Makka", numberOfVerses: 96, revelationOrder: 46);
        _Chapters[57] = new Chapter(chapterNumber: 57, arabicName: "الحديد", englishName: "Al Hadid (Iron)", period: "Madina", numberOfVerses: 29, revelationOrder: 94);
        _Chapters[58] = new Chapter(chapterNumber: 58, arabicName: "المجادلة", englishName: "Al Mujadila (The pleading woman)", period: "Madina", numberOfVerses: 22, revelationOrder: 105);
        _Chapters[59] = new Chapter(chapterNumber: 59, arabicName: "الحشر", englishName: "Al Hashr (Exile)", period: "Madina", numberOfVerses: 24, revelationOrder: 101);
        _Chapters[60] = new Chapter(chapterNumber: 60, arabicName: "الممتحنة", englishName: "Al Mumtahana (The examined woman)", period: "Madina", numberOfVerses: 13, revelationOrder: 91);
        _Chapters[61] = new Chapter(chapterNumber: 61, arabicName: "الصف", englishName: "As Saff (The ranks)", period: "Madina", numberOfVerses: 14, revelationOrder: 109);
        _Chapters[62] = new Chapter(chapterNumber: 62, arabicName: "الجمعة", englishName: "Al Jumua (The congregation)", period: "Madina", numberOfVerses: 11, revelationOrder: 110);
        _Chapters[63] = new Chapter(chapterNumber: 63, arabicName: "المنافقون", englishName: "Al Munafiqun (The hypocrites)", period: "Madina", numberOfVerses: 11, revelationOrder: 104);
        _Chapters[64] = new Chapter(chapterNumber: 64, arabicName: "التغابن", englishName: "At Taghabun (Manifestation of losses)", period: "Makka", numberOfVerses: 18, revelationOrder: 108);
        _Chapters[65] = new Chapter(chapterNumber: 65, arabicName: "الطلاق", englishName: "At Talaq (Divorce)", period: "Madina", numberOfVerses: 12, revelationOrder: 99);
        _Chapters[66] = new Chapter(chapterNumber: 66, arabicName: "التحريم", englishName: "At Tahrim (Prohibition)", period: "Madina", numberOfVerses: 12, revelationOrder: 107);
        _Chapters[67] = new Chapter(chapterNumber: 67, arabicName: "الملك", englishName: "Al Mulk (The kingdom)", period: "Makka", numberOfVerses: 30, revelationOrder: 77);
        _Chapters[68] = new Chapter(chapterNumber: 68, arabicName: "القلم", englishName: "Al Qalam (The pen)", period: "Makka", numberOfVerses: 52, revelationOrder: 2);
        _Chapters[69] = new Chapter(chapterNumber: 69, arabicName: "الحاقة", englishName: "Al Haqqa (The reality)", period: "Makka", numberOfVerses: 52, revelationOrder: 78);
        _Chapters[70] = new Chapter(chapterNumber: 70, arabicName: "المعارج", englishName: "Al Maarij (Ascending stairways)", period: "Makka", numberOfVerses: 44, revelationOrder: 79);
        _Chapters[71] = new Chapter(chapterNumber: 71, arabicName: "نوح", englishName: "Nuh (Noah)", period: "Makka", numberOfVerses: 28, revelationOrder: 71);
        _Chapters[72] = new Chapter(chapterNumber: 72, arabicName: "الجن", englishName: "Al Jinn (The Jinn)", period: "Makka", numberOfVerses: 28, revelationOrder: 40);
        _Chapters[73] = new Chapter(chapterNumber: 73, arabicName: "المزمل", englishName: "Al Muzzammil (The enshrouded one)", period: "Makka", numberOfVerses: 20, revelationOrder: 3);
        _Chapters[74] = new Chapter(chapterNumber: 74, arabicName: "المدثر", englishName: "Al Muddaththir (The cloaked one)", period: "Makka", numberOfVerses: 56, revelationOrder: 4);
        _Chapters[75] = new Chapter(chapterNumber: 75, arabicName: "القيامة", englishName: "Al Qiyama (Resurrection)", period: "Makka", numberOfVerses: 40, revelationOrder: 31);
        _Chapters[76] = new Chapter(chapterNumber: 76, arabicName: "الانسان", englishName: "Al Insan (Man)", period: "Makka", numberOfVerses: 31, revelationOrder: 98);
        _Chapters[77] = new Chapter(chapterNumber: 77, arabicName: "المرسلات", englishName: "Al Mursalat (The emissaries)", period: "Makka", numberOfVerses: 50, revelationOrder: 33);
        _Chapters[78] = new Chapter(chapterNumber: 78, arabicName: "النبإ", englishName: "An Naba (The tidings)", period: "Makka", numberOfVerses: 40, revelationOrder: 80);
        _Chapters[79] = new Chapter(chapterNumber: 79, arabicName: "النازعات", englishName: "An Naziat (Those who yearn)", period: "Makka", numberOfVerses: 46, revelationOrder: 81);
        _Chapters[80] = new Chapter(chapterNumber: 80, arabicName: "عبس", englishName: "Abasa (He frowned)", period: "Makka", numberOfVerses: 42, revelationOrder: 24);
        _Chapters[81] = new Chapter(chapterNumber: 81, arabicName: "التكوير", englishName: "At Takwir (The folding up)", period: "Makka", numberOfVerses: 29, revelationOrder: 7);
        _Chapters[82] = new Chapter(chapterNumber: 82, arabicName: "الإنفطار", englishName: "Al Infitar (The cleaving)", period: "Makka", numberOfVerses: 19, revelationOrder: 82);
        _Chapters[83] = new Chapter(chapterNumber: 83, arabicName: "المطففين", englishName: "Al Mutaffifin (The cheats)", period: "Makka", numberOfVerses: 36, revelationOrder: 86);
        _Chapters[84] = new Chapter(chapterNumber: 84, arabicName: "الإنشقاق", englishName: "Al Isnhiqaq (Splitting open)", period: "Makka", numberOfVerses: 25, revelationOrder: 83);
        _Chapters[85] = new Chapter(chapterNumber: 85, arabicName: "البروج", englishName: "Al Buruj (The stars)", period: "Makka", numberOfVerses: 22, revelationOrder: 27);
        _Chapters[86] = new Chapter(chapterNumber: 86, arabicName: "الطارق", englishName: "At Tariq (The morning star)", period: "Makka", numberOfVerses: 17, revelationOrder: 36);
        _Chapters[87] = new Chapter(chapterNumber: 87, arabicName: "الأعلى", englishName: "Al Ala (The most high)", period: "Makka", numberOfVerses: 19, revelationOrder: 8);
        _Chapters[88] = new Chapter(chapterNumber: 88, arabicName: "الغاشية", englishName: "Al Ghashiya (The overwhelming)", period: "Makka", numberOfVerses: 26, revelationOrder: 68);
        _Chapters[89] = new Chapter(chapterNumber: 89, arabicName: "الفجر", englishName: "Al Fajr (The dawn)", period: "Makka", numberOfVerses: 30, revelationOrder: 10);
        _Chapters[90] = new Chapter(chapterNumber: 90, arabicName: "البلد", englishName: "Al Balad (The city)", period: "Makka", numberOfVerses: 20, revelationOrder: 35);
        _Chapters[91] = new Chapter(chapterNumber: 91, arabicName: "الشمس", englishName: "Ash Shams (The Sun)", period: "Makka", numberOfVerses: 15, revelationOrder: 26);
        _Chapters[92] = new Chapter(chapterNumber: 92, arabicName: "الليل", englishName: "Al Lail (The night)", period: "Makka", numberOfVerses: 21, revelationOrder: 9);
        _Chapters[93] = new Chapter(chapterNumber: 93, arabicName: "الضحى", englishName: "Ad Duhaa (The brightness of day)", period: "Makka", numberOfVerses: 11, revelationOrder: 11);
        _Chapters[94] = new Chapter(chapterNumber: 94, arabicName: "الشرح", englishName: "Al Inshirah (The expansion)", period: "Makka", numberOfVerses: 8, revelationOrder: 12);
        _Chapters[95] = new Chapter(chapterNumber: 95, arabicName: "التين", englishName: "At tin (The fig)", period: "Makka", numberOfVerses: 8, revelationOrder: 28);
        _Chapters[96] = new Chapter(chapterNumber: 96, arabicName: "العلق", englishName: "Al Alaq (The clot)", period: "Makka", numberOfVerses: 19, revelationOrder: 1);
        _Chapters[97] = new Chapter(chapterNumber: 97, arabicName: "القدر", englishName: "Al Qadr (The power)", period: "Makka", numberOfVerses: 5, revelationOrder: 25);
        _Chapters[98] = new Chapter(chapterNumber: 98, arabicName: "البينة", englishName: "Al Bayyina (Clear proof)", period: "Madina", numberOfVerses: 8, revelationOrder: 100);
        _Chapters[99] = new Chapter(chapterNumber: 99, arabicName: "الزلزلة", englishName: "Al Zalzala (The shaking)", period: "Makka", numberOfVerses: 8, revelationOrder: 93);
        _Chapters[100] = new Chapter(chapterNumber: 100, arabicName: "العاديات", englishName: "Al Adiyat (The assaulters)", period: "Makka", numberOfVerses: 11, revelationOrder: 14);
        _Chapters[101] = new Chapter(chapterNumber: 101, arabicName: "القارعة", englishName: "Al Qaria (The calamity)", period: "Makka", numberOfVerses: 11, revelationOrder: 30);
        _Chapters[102] = new Chapter(chapterNumber: 102, arabicName: "التكاثر", englishName: "At Takathur (Abundance of wealth)", period: "Makka", numberOfVerses: 8, revelationOrder: 16);
        _Chapters[103] = new Chapter(chapterNumber: 103, arabicName: "العصر", englishName: "Al Asr (The time)", period: "Makka", numberOfVerses: 3, revelationOrder: 13);
        _Chapters[104] = new Chapter(chapterNumber: 104, arabicName: "الهمزة", englishName: "Al Humaza (The slanderer)", period: "Makka", numberOfVerses: 9, revelationOrder: 32);
        _Chapters[105] = new Chapter(chapterNumber: 105, arabicName: "الفيل", englishName: "Al Fil (The elephant)", period: "Makka", numberOfVerses: 5, revelationOrder: 19);
        _Chapters[106] = new Chapter(chapterNumber: 106, arabicName: "قريش", englishName: "Quraish (The Quraish)", period: "Makka", numberOfVerses: 4, revelationOrder: 29);
        _Chapters[107] = new Chapter(chapterNumber: 107, arabicName: "الماعون", englishName: "Al Maun (Almsgiving)", period: "Makka", numberOfVerses: 7, revelationOrder: 17);
        _Chapters[108] = new Chapter(chapterNumber: 108, arabicName: "الكوثر", englishName: "Al Kauthar (Abundance)", period: "Makka", numberOfVerses: 3, revelationOrder: 15);
        _Chapters[109] = new Chapter(chapterNumber: 109, arabicName: "الكافرون", englishName: "Al Kafirun (The disbelievers)", period: "Makka", numberOfVerses: 6, revelationOrder: 18);
        _Chapters[110] = new Chapter(chapterNumber: 110, arabicName: "النصر", englishName: "An Nasr (Divine support)", period: "Madina", numberOfVerses: 3, revelationOrder: 114);
        _Chapters[111] = new Chapter(chapterNumber: 111, arabicName: "المسد", englishName: "Al Lahab (The flame)", period: "Makka", numberOfVerses: 5, revelationOrder: 6);
        _Chapters[112] = new Chapter(chapterNumber: 112, arabicName: "الإخلاص", englishName: "Al Ikhlas (The unity)", period: "Makka", numberOfVerses: 4, revelationOrder: 22);
        _Chapters[113] = new Chapter(chapterNumber: 113, arabicName: "الفلق", englishName: "Al Falaq (Dawn)", period: "Makka", numberOfVerses: 5, revelationOrder: 20);
        _Chapters[114] = new Chapter(chapterNumber: 114, arabicName: "الناس", englishName: "An Nas (Mankind)", period: "Makka", numberOfVerses: 6, revelationOrder: 21);
    }
}
