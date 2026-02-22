using System.Collections.Frozen;
using System.Collections.Immutable;

namespace QuranX.Shared.Models;

public static class QuranStructure
{
    public static ImmutableArray<ChapterData> Chapters { get; }
    private static readonly FrozenDictionary<int, ChapterData> ChapterLookup;

    static QuranStructure()
    {
        Chapters = GenerateChaptersData();
        ChapterLookup = Chapters.ToFrozenDictionary(x => x.ChapterNumber);
    }

    public static ChapterData Chapter(int chapterNumber) => ChapterLookup[chapterNumber];
    public static int VerseCount(int chapter) => ChapterLookup[chapter].NumberOfVerses;

    public static bool TryValidateChapterAndVerse(int chapterNumber, int verseNumber)
    {
        if (chapterNumber < 1 || chapterNumber > 114)
            return false;
        if (verseNumber < 1 || verseNumber > Chapter(chapterNumber).NumberOfVerses)
            return false;
        return true;
    }

    private static ImmutableArray<ChapterData> GenerateChaptersData()
    {
        ChapterData[] result = [
            new ChapterData(1,
                arabicName: "الفاتحة",
                englishName: "Al-Faatiha (The Opening)",
                numberOfVerses: 7,
                revelationOrder: 5),

            new ChapterData(2,
                arabicName: "البقرة",
                englishName: "Al-Baqara (The Cow)",
                numberOfVerses: 286,
                revelationOrder: 87),

            new ChapterData(3,
                arabicName: "آل عمران",
                englishName: "Aal-i-Imraan (The Family of Imraan)",
                numberOfVerses: 200,
                revelationOrder: 89),

            new ChapterData(4,
                arabicName: "النساء",
                englishName: "An-Nisaa (The Women)",
                numberOfVerses: 176,
                revelationOrder: 92),

            new ChapterData(5,
                arabicName: "المائدة",
                englishName: "Al-Maaida (The Table)",
                numberOfVerses: 120,
                revelationOrder: 112),

            new ChapterData(6,
                arabicName: "الأنعام",
                englishName: "Al-An'aam (The Cattle)",
                numberOfVerses: 165,
                revelationOrder: 55),

            new ChapterData(7,
                arabicName: "الأعراف",
                englishName: "Al-A'raaf (The Heights)",
                numberOfVerses: 206,
                revelationOrder: 39),

            new ChapterData(8,
                arabicName: "الأنفال",
                englishName: "Al-Anfaal (The Spoils of War)",
                numberOfVerses: 75,
                revelationOrder: 88),

            new ChapterData(9,
                arabicName: "التوبة",
                englishName: "At-Tawba (The Repentance)",
                numberOfVerses: 129,
                revelationOrder: 113),

            new ChapterData(10,
                arabicName: "يونس",
                englishName: "Yunus (Jonah)",
                numberOfVerses: 109,
                revelationOrder: 51),

            new ChapterData(11,
                arabicName: "هود",
                englishName: "Hud (Hud)",
                numberOfVerses: 123,
                revelationOrder: 52),

            new ChapterData(12,
                arabicName: "يوسف",
                englishName: "Yusuf (Joseph)",
                numberOfVerses: 111,
                revelationOrder: 53),

            new ChapterData(13,
                arabicName: "الرعد",
                englishName: "Ar-Ra'd (The Thunder)",
                numberOfVerses: 43,
                revelationOrder: 96),

            new ChapterData(14,
                arabicName: "ابراهيم",
                englishName: "Ibrahim (Abraham)",
                numberOfVerses: 52,
                revelationOrder: 72),

            new ChapterData(15,
                arabicName: "الحجر",
                englishName: "Al-Hijr (The Rock)",
                numberOfVerses: 99,
                revelationOrder: 54),

            new ChapterData(16,
                arabicName: "النحل",
                englishName: "An-Nahl (The Bee)",
                numberOfVerses: 128,
                revelationOrder: 70),

            new ChapterData(17,
                arabicName: "الإسراء",
                englishName: "Al-Israa (The Night Journey)",
                numberOfVerses: 111,
                revelationOrder: 50),

            new ChapterData(18,
                arabicName: "الكهف",
                englishName: "Al-Kahf (The Cave)",
                numberOfVerses: 110,
                revelationOrder: 69),

            new ChapterData(19,
                arabicName: "مريم",
                englishName: "Maryam (Mary)",
                numberOfVerses: 98,
                revelationOrder: 44),

            new ChapterData(20,
                arabicName: "طه",
                englishName: "Taa-Haa (Taa-Haa)",
                numberOfVerses: 135,
                revelationOrder: 45),

            new ChapterData(21,
                arabicName: "الأنبياء",
                englishName: "Al-Anbiyaa (The Prophets)",
                numberOfVerses: 112,
                revelationOrder: 73),

            new ChapterData(22,
                arabicName: "الحج",
                englishName: "Al-Hajj (The Pilgrimage)",
                numberOfVerses: 78,
                revelationOrder: 103),

            new ChapterData(23,
                arabicName: "المؤمنون",
                englishName: "Al-Mu'minoon (The Believers)",
                numberOfVerses: 118,
                revelationOrder: 74),

            new ChapterData(24,
                arabicName: "النور",
                englishName: "An-Noor (The Light)",
                numberOfVerses: 64,
                revelationOrder: 102),

            new ChapterData(25,
                arabicName: "الفرقان",
                englishName: "Al-Furqaan (The Criterion)",
                numberOfVerses: 77,
                revelationOrder: 42),

            new ChapterData(26,
                arabicName: "الشعراء",
                englishName: "Ash-Shu'araa (The Poets)",
                numberOfVerses: 227,
                revelationOrder: 47),

            new ChapterData(27,
                arabicName: "النمل",
                englishName: "An-Naml (The Ant)",
                numberOfVerses: 93,
                revelationOrder: 48),

            new ChapterData(28,
                arabicName: "القصص",
                englishName: "Al-Qasas (The Stories)",
                numberOfVerses: 88,
                revelationOrder: 49),

            new ChapterData(29,
                arabicName: "العنكبوت",
                englishName: "Al-Ankaboot (The Spider)",
                numberOfVerses: 69,
                revelationOrder: 85),

            new ChapterData(30,
                arabicName: "الروم",
                englishName: "Ar-Room (The Romans)",
                numberOfVerses: 60,
                revelationOrder: 84),

            new ChapterData(31,
                arabicName: "لقمان",
                englishName: "Luqman (Luqman)",
                numberOfVerses: 34,
                revelationOrder: 57),

            new ChapterData(32,
                arabicName: "السجدة",
                englishName: "As-Sajda (The Prostration)",
                numberOfVerses: 30,
                revelationOrder: 75),

            new ChapterData(33,
                arabicName: "الأحزاب",
                englishName: "Al-Ahzaab (The Confederates)",
                numberOfVerses: 73,
                revelationOrder: 90),

            new ChapterData(34,
                arabicName: "سبإ",
                englishName: "Saba (Sheba)",
                numberOfVerses: 54,
                revelationOrder: 58),

            new ChapterData(35,
                arabicName: "فاطر",
                englishName: "Faatir (The Originator)",
                numberOfVerses: 45,
                revelationOrder: 43),

            new ChapterData(36,
                arabicName: "يس",
                englishName: "Yaseen (Yaseen)",
                numberOfVerses: 83,
                revelationOrder: 41),

            new ChapterData(37,
                arabicName: "الصافات",
                englishName: "As-Saaffaat (Those Ranged in Ranks)",
                numberOfVerses: 182,
                revelationOrder: 56),

            new ChapterData(38,
                arabicName: "ص",
                englishName: "Saad (The Letter Saad)",
                numberOfVerses: 88,
                revelationOrder: 38),

            new ChapterData(39,
                arabicName: "الزمر",
                englishName: "Az-Zumar (The Troops)",
                numberOfVerses: 75,
                revelationOrder: 59),

            new ChapterData(40,
                arabicName: "غافر",
                englishName: "Ghaafir (The Forgiver)",
                numberOfVerses: 85,
                revelationOrder: 60),

            new ChapterData(41,
                arabicName: "فصلت",
                englishName: "Fussilat (Explained in Detail)",
                numberOfVerses: 54,
                revelationOrder: 61),

            new ChapterData(42,
                arabicName: "الشورى",
                englishName: "Ash-Shura (Consultation)",
                numberOfVerses: 53,
                revelationOrder: 62),

            new ChapterData(43,
                arabicName: "الزخرف",
                englishName: "Az-Zukhruf (Ornaments of Gold)",
                numberOfVerses: 89,
                revelationOrder: 63),

            new ChapterData(44,
                arabicName: "الدخان",
                englishName: "Ad-Dukhaan (The Smoke)",
                numberOfVerses: 59,
                revelationOrder: 64),

            new ChapterData(45,
                arabicName: "الجاثية",
                englishName: "Al-Jaathiya (Crouching)",
                numberOfVerses: 37,
                revelationOrder: 65),

            new ChapterData(46,
                arabicName: "الأحقاف",
                englishName: "Al-Ahqaf (The Dunes)",
                numberOfVerses: 35,
                revelationOrder: 66),

            new ChapterData(47,
                arabicName: "محمد",
                englishName: "Muhammad (Muhammad)",
                numberOfVerses: 38,
                revelationOrder: 95),

            new ChapterData(48,
                arabicName: "الفتح",
                englishName: "Al-Fath (The Victory)",
                numberOfVerses: 29,
                revelationOrder: 111),

            new ChapterData(49,
                arabicName: "الحجرات",
                englishName: "Al-Hujuraat (The Inner Apartments)",
                numberOfVerses: 18,
                revelationOrder: 106),

            new ChapterData(50,
                arabicName: "ق",
                englishName: "Qaaf (The Letter Qaaf)",
                numberOfVerses: 45,
                revelationOrder: 34),

            new ChapterData(51,
                arabicName: "الذاريات",
                englishName: "Adh-Dhaariyat (The Winnowing Winds)",
                numberOfVerses: 60,
                revelationOrder: 67),

            new ChapterData(52,
                arabicName: "الطور",
                englishName: "At-Tur (The Mount)",
                numberOfVerses: 49,
                revelationOrder: 76),

            new ChapterData(53,
                arabicName: "النجم",
                englishName: "An-Najm (The Star)",
                numberOfVerses: 62,
                revelationOrder: 23),

            new ChapterData(54,
                arabicName: "القمر",
                englishName: "Al-Qamar (The Moon)",
                numberOfVerses: 55,
                revelationOrder: 37),

            new ChapterData(55,
                arabicName: "الرحمن",
                englishName: "Ar-Rahmaan (The Beneficent)",
                numberOfVerses: 78,
                revelationOrder: 97),

            new ChapterData(56,
                arabicName: "الواقعة",
                englishName: "Al-Waaqi'a (The Inevitable)",
                numberOfVerses: 96,
                revelationOrder: 46),

            new ChapterData(57,
                arabicName: "الحديد",
                englishName: "Al-Hadid (The Iron)",
                numberOfVerses: 29,
                revelationOrder: 94),

            new ChapterData(58,
                arabicName: "المجادلة",
                englishName: "Al-Mujaadila (The Pleading Woman)",
                numberOfVerses: 22,
                revelationOrder: 105),

            new ChapterData(59,
                arabicName: "الحشر",
                englishName: "Al-Hashr (The Exile)",
                numberOfVerses: 24,
                revelationOrder: 101),

            new ChapterData(60,
                arabicName: "الممتحنة",
                englishName: "Al-Mumtahana (She that is to be Examined)",
                numberOfVerses: 13,
                revelationOrder: 91),

            new ChapterData(61,
                arabicName: "الصف",
                englishName: "As-Saff (The Ranks)",
                numberOfVerses: 14,
                revelationOrder: 109),

            new ChapterData(62,
                arabicName: "الجمعة",
                englishName: "Al-Jumu'a (Friday)",
                numberOfVerses: 11,
                revelationOrder: 110),

            new ChapterData(63,
                arabicName: "المنافقون",
                englishName: "Al-Munaafiqoon (The Hypocrites)",
                numberOfVerses: 11,
                revelationOrder: 104),

            new ChapterData(64,
                arabicName: "التغابن",
                englishName: "At-Taghaabun (Mutual Disillusion)",
                numberOfVerses: 18,
                revelationOrder: 108),

            new ChapterData(65,
                arabicName: "الطلاق",
                englishName: "At-Talaaq (Divorce)",
                numberOfVerses: 12,
                revelationOrder: 99),

            new ChapterData(66,
                arabicName: "التحريم",
                englishName: "At-Tahrim (The Prohibition)",
                numberOfVerses: 12,
                revelationOrder: 107),

            new ChapterData(67,
                arabicName: "الملك",
                englishName: "Al-Mulk (The Sovereignty)",
                numberOfVerses: 30,
                revelationOrder: 77),

            new ChapterData(68,
                arabicName: "القلم",
                englishName: "Al-Qalam (The Pen)",
                numberOfVerses: 52,
                revelationOrder: 2),

            new ChapterData(69,
                arabicName: "الحاقة",
                englishName: "Al-Haaqqa (The Reality)",
                numberOfVerses: 52,
                revelationOrder: 78),

            new ChapterData(70,
                arabicName: "المعارج",
                englishName: "Al-Ma'aarij (The Ascending Stairways)",
                numberOfVerses: 44,
                revelationOrder: 79),

            new ChapterData(71,
                arabicName: "نوح",
                englishName: "Nuh (Noah)",
                numberOfVerses: 28,
                revelationOrder: 71),

            new ChapterData(72,
                arabicName: "الجن",
                englishName: "Al-Jinn (The Jinn)",
                numberOfVerses: 28,
                revelationOrder: 40),

            new ChapterData(73,
                arabicName: "المزمل",
                englishName: "Al-Muzzammil (The Enshrouded One)",
                numberOfVerses: 20,
                revelationOrder: 3),

            new ChapterData(74,
                arabicName: "المدثر",
                englishName: "Al-Muddaththir (The Cloaked One)",
                numberOfVerses: 56,
                revelationOrder: 4),

            new ChapterData(75,
                arabicName: "القيامة",
                englishName: "Al-Qiyaama (The Resurrection)",
                numberOfVerses: 40,
                revelationOrder: 31),

            new ChapterData(76,
                arabicName: "الانسان",
                englishName: "Al-Insaan (Man)",
                numberOfVerses: 31,
                revelationOrder: 98),

            new ChapterData(77,
                arabicName: "المرسلات",
                englishName: "Al-Mursalaat (The Emissaries)",
                numberOfVerses: 50,
                revelationOrder: 33),

            new ChapterData(78,
                arabicName: "النبإ",
                englishName: "An-Naba (The Tidings)",
                numberOfVerses: 40,
                revelationOrder: 80),

            new ChapterData(79,
                arabicName: "النازعات",
                englishName: "An-Naazi'aat (Those Who Drag Forth)",
                numberOfVerses: 46,
                revelationOrder: 81),

            new ChapterData(80,
                arabicName: "عبس",
                englishName: "'Abasa (He Frowned)",
                numberOfVerses: 42,
                revelationOrder: 24),

            new ChapterData(81,
                arabicName: "التكوير",
                englishName: "At-Takwir (The Overthrowing)",
                numberOfVerses: 29,
                revelationOrder: 7),

            new ChapterData(82,
                arabicName: "الإنفطار",
                englishName: "Al-Infitaar (The Cleaving)",
                numberOfVerses: 19,
                revelationOrder: 82),

            new ChapterData(83,
                arabicName: "المطففين",
                englishName: "Al-Mutaffifin (Defrauding)",
                numberOfVerses: 36,
                revelationOrder: 86),

            new ChapterData(84,
                arabicName: "الإنشقاق",
                englishName: "Al-Inshiqaaq (The Splitting Open)",
                numberOfVerses: 25,
                revelationOrder: 83),

            new ChapterData(85,
                arabicName: "البروج",
                englishName: "Al-Burooj (The Mansions of the Stars)",
                numberOfVerses: 22,
                revelationOrder: 27),

            new ChapterData(86,
                arabicName: "الطارق",
                englishName: "At-Taariq (The Morning Star)",
                numberOfVerses: 17,
                revelationOrder: 36),

            new ChapterData(87,
                arabicName: "الأعلى",
                englishName: "Al-A'laa (The Most High)",
                numberOfVerses: 19,
                revelationOrder: 8),

            new ChapterData(88,
                arabicName: "الغاشية",
                englishName: "Al-Ghaashiya (The Overwhelming)",
                numberOfVerses: 26,
                revelationOrder: 68),

            new ChapterData(89,
                arabicName: "الفجر",
                englishName: "Al-Fajr (The Dawn)",
                numberOfVerses: 30,
                revelationOrder: 10),

            new ChapterData(90,
                arabicName: "البلد",
                englishName: "Al-Balad (The City)",
                numberOfVerses: 20,
                revelationOrder: 35),

            new ChapterData(91,
                arabicName: "الشمس",
                englishName: "Ash-Shams (The Sun)",
                numberOfVerses: 15,
                revelationOrder: 26),

            new ChapterData(92,
                arabicName: "الليل",
                englishName: "Al-Lail (The Night)",
                numberOfVerses: 21,
                revelationOrder: 9),

            new ChapterData(93,
                arabicName: "الضحى",
                englishName: "Ad-Dhuhaa (The Morning Hours)",
                numberOfVerses: 11,
                revelationOrder: 11),

            new ChapterData(94,
                arabicName: "الشرح",
                englishName: "Ash-Sharh (The Relief)",
                numberOfVerses: 8,
                revelationOrder: 12),

            new ChapterData(95,
                arabicName: "التين",
                englishName: "At-Tin (The Fig)",
                numberOfVerses: 8,
                revelationOrder: 28),

            new ChapterData(96,
                arabicName: "العلق",
                englishName: "Al-'Alaq (The Clot)",
                numberOfVerses: 19,
                revelationOrder: 1),

            new ChapterData(97,
                arabicName: "القدر",
                englishName: "Al-Qadr (The Power)",
                numberOfVerses: 5,
                revelationOrder: 25),

            new ChapterData(98,
                arabicName: "البينة",
                englishName: "Al-Bayyina (The Clear Proof)",
                numberOfVerses: 8,
                revelationOrder: 100),

            new ChapterData(99,
                arabicName: "الزلزلة",
                englishName: "Az-Zalzala (The Earthquake)",
                numberOfVerses: 8,
                revelationOrder: 93),

            new ChapterData(100,
                arabicName: "العاديات",
                englishName: "Al-'Aadiyaat (The Courser)",
                numberOfVerses: 11,
                revelationOrder: 14),

            new ChapterData(101,
                arabicName: "القارعة",
                englishName: "Al-Qaari'a (The Calamity)",
                numberOfVerses: 11,
                revelationOrder: 30),

            new ChapterData(102,
                arabicName: "التكاثر",
                englishName: "At-Takaathur (Rivalry in Worldly Increase)",
                numberOfVerses: 8,
                revelationOrder: 16),

            new ChapterData(103,
                arabicName: "العصر",
                englishName: "Al-'Asr (The Declining Day)",
                numberOfVerses: 3,
                revelationOrder: 13),

            new ChapterData(104,
                arabicName: "الهمزة",
                englishName: "Al-Humaza (The Traducer)",
                numberOfVerses: 9,
                revelationOrder: 32),

            new ChapterData(105,
                arabicName: "الفيل",
                englishName: "Al-Fil (The Elephant)",
                numberOfVerses: 5,
                revelationOrder: 19),

            new ChapterData(106,
                arabicName: "قريش",
                englishName: "Quraish (Quraish)",
                numberOfVerses: 4,
                revelationOrder: 29),

            new ChapterData(107,
                arabicName: "الماعون",
                englishName: "Al-Maa'un (Almsgiving)",
                numberOfVerses: 7,
                revelationOrder: 17),

            new ChapterData(108,
                arabicName: "الكوثر",
                englishName: "Al-Kawthar (Abundance)",
                numberOfVerses: 3,
                revelationOrder: 15),

            new ChapterData(109,
                arabicName: "الكافرون",
                englishName: "Al-Kaafiroon (The Disbelievers)",
                numberOfVerses: 6,
                revelationOrder: 18),

            new ChapterData(110,
                arabicName: "النصر",
                englishName: "An-Nasr (Divine Support)",
                numberOfVerses: 3,
                revelationOrder: 114),

            new ChapterData(111,
                arabicName: "المسد",
                englishName: "Al-Masad (The Palm Fiber)",
                numberOfVerses: 5,
                revelationOrder: 6),

            new ChapterData(112,
                arabicName: "الإخلاص",
                englishName: "Al-Ikhlaas (Sincerity)",
                numberOfVerses: 4,
                revelationOrder: 22),

            new ChapterData(113,
                arabicName: "الفلق",
                englishName: "Al-Falaq (The Dawn)",
                numberOfVerses: 5,
                revelationOrder: 20),

            new ChapterData(114,
                arabicName: "الناس",
                englishName: "An-Naas (Mankind)",
                numberOfVerses: 6,
                revelationOrder: 21),
        ];

        return result.ToImmutableArray();
    }

}
