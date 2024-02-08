using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using The49.Maui.BottomSheet;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace SpiritsFirstTry.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        public MapView MainMapView { get; set; }
        public static List<Spirit> Spirits { get; set; }
        BottomSheetView bottomSheeet;
        BottomSheetViewModel bottomSheeetVm;

        public MainViewModel() {
            LoadSpirits();

            bottomSheeetVm = new BottomSheetViewModel();
            bottomSheeetVm.spiritList = Spirits;
            bottomSheeet = new BottomSheetView(bottomSheeetVm);
            bottomSheeet.IsCancelable = false;
            bottomSheeet.Detents.Add(new MediumDetent());
            bottomSheeet.Detents.Add(new FullscreenDetent());

            //     bottomSheeet.SelectedDetent = bottomSheeet.Detents[2];

        }

        public void SetupMap(MapView mapView)
        {
            MainMapView = mapView;
            bottomSheeetVm.mapView = MainMapView;
            MainMapView.Map = new Map(BasemapStyle.OSMNavigationDark);

            var MinskPoint = new MapPoint(27.542744, 53.897867, SpatialReferences.Wgs84);

            MainMapView.Map.InitialViewpoint = new Viewpoint(MinskPoint, 10000000);

            _ = Initialize();
        }

        private async Task Initialize()
        {

            try
            {
                await CreateSpiritMarkers();
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }


            try
            {
                this.MainMapView.GeoViewTapped += GeoViewTapped;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }
        }

        public void OpenSearch()
        {
            if (bottomSheeet.Detents.Count < 3)
            {
                var ZeroDetent = new RatioDetent();
                ZeroDetent.Ratio = 0.1f;
                bottomSheeet.Detents.Add(ZeroDetent);
            }
            bottomSheeetVm.IsSpiritOpend = false;
            bottomSheeetVm.IsSearchOpend = true;

            if (!bottomSheeet.IsLoaded)
            {
                bottomSheeet.ShowAsync();
            }

            bottomSheeet.SelectedDetent = bottomSheeet.Detents[0];

        }
            private async void GeoViewTapped(object sender, Esri.ArcGISRuntime.Maui.GeoViewInputEventArgs e)
        {

            MapPoint tappedLocation = (MapPoint)e.Location.NormalizeCentralMeridian();
            MapPoint MinskLocation = new MapPoint(27.542744, 53.897867, SpatialReferences.Wgs84);
            // var dist = tappedLocation.Distance(MinskLocation.Project(SpatialReferences.WebMercator));
            //Console.WriteLine(dist/MainMapView.MapScale*100);

            double min_dist = double.MaxValue;
            Spirit closest = null;
            foreach (var spirit in Spirits)
            {
                var dist = tappedLocation.Distance(spirit.mapPoint);
                if (dist < min_dist)
                {
                    min_dist = dist;
                    closest = spirit;
                }
            }
            if (bottomSheeet.Detents.Count < 3)
            {
                var ZeroDetent = new RatioDetent();
                ZeroDetent.Ratio = 0.1f;
                bottomSheeet.Detents.Add(ZeroDetent);
            }
            if (min_dist / MainMapView.MapScale * 100 < 1)
            {
                bottomSheeetVm.IsSpiritOpend = true;
                bottomSheeetVm.IsSearchOpend = false;




                foreach(var spirit in Spirits)
                {
                    spirit.markerSymbol.Height = 40;
                    spirit.markerSymbol.Width = 40;
                }
                closest.markerSymbol.Height = 60;
                closest.markerSymbol.Width = 60;
                closest.pinGraphic.ZIndex = Spirit.maxzindex + 1;

                Console.WriteLine(closest.Name);
                bottomSheeetVm.SetSelectedSpirit(closest);
                if (!bottomSheeet.IsLoaded)
                {
                    bottomSheeet.ShowAsync();
                }

                bottomSheeet.SelectedDetent = bottomSheeet.Detents[0];
                MapPoint mapPoint = new MapPoint(closest.mapPoint.X, closest.mapPoint.Y - 200000, closest.mapPoint.SpatialReference);
                Viewpoint viewpoint = new Viewpoint(mapPoint, 5000000);
                await this.MainMapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(0.5));
            }
            else
            {

                 bottomSheeet.SelectedDetent = bottomSheeet.Detents[2];
            }
        }


        public void LoadSpirits()
        {
            Spirits = new List<Spirit>();

            Spirits.Add(new Spirit(27.056924, 53.967102,
                "Зазоўка",
                "Зазоўкі завабліваюць мужчын у гушчар сваёй прыгажосцю ды галіз­ной. Толькі даўгія валасы прыкрываюць самыя далікатныя часткі цела – што, вядома, пры­ваблівае мужчын яшчэ больш. Зазоўкі клічуць мужчын на імя, абяцаюць незвычай­ныя адчуванні. Хтосьці трапляе ў багну ці ў пастку, бо крочыць за прыгажуняй, не разбіраючы шляху. Хтосьці атрымлі­\r\nвае жаданае.\r\nТолькі пасля любошчаў Зазоўкі\r\nмужчына вяртаецца дадому не зусім сабой. Ён ужо не будзе жыць, як ра­ ней. З часам зноў сыдзе ў лес на по­ шукі чароўнай красуні. І болей ужо не вернецца.\r\n"
                , "zazouka.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Ракаў" }));

            Spirits.Add(new Spirit(27.090737, 53.151117,
                "Аднарог",
                "Апошні аднарог у Еўропе быў упаляваны менавіта ў Беларусі. Упаляваць аднарога можна толькі прывабіў­шы яго спевам юнай нявінніцы.\r\nРог аднарога, як вядома, мог лекаваць раны ды хваробы, ратаваць ад атруты.\r\n"
                , "adnaroh.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Копыль " }));

            Spirits.Add(new Spirit(27.097120, 53.154631,
                "Азярніцы",
                "АЗЯРНіЦЫ, міфічныя насельніцы Чорнага возера, што знаходзіцца у лесе, недалёка ад вёскі Брусы Мядзельскага р-на. Уяуляліся у выглядзе маладых жанчын з доугімі зеленаватымі валасамі, цемнай скураю і ступнямі ў выглядзе плаўнікоў.\r\nНа дотык вельмі халодныя і замест крыві ў іх нібыта вада. Апранутыя ў  сукні, сплеценыя з багавіння. Выходвячы на бераг, размауляюць на незразумелай, як быццам птушынай,\r\nмове і спяваюць падобна да салоўкаў.\r\nЧалавек, які ўбачыць азярніц, не павінен выдаць сябе, каб не быць зацягнутым імі ў  багну. Гэта награжае і тым, хто купаецца ў  возеры. Азярніцы сваіх ахвар не адпускаюць.\r\n"
                , "aziarnizy.png",
                new List<SpiritsClassification> { SpiritsClassification.water },
                new List<string> { "веска Брусы, Копыльскі раен" }));

            Spirits.Add(new Spirit(26.907148, 54.855610,
                "Апівень",
                "АПІВЕНЬ, нячысцік, які чапляецца да людей, схіляючы іх да пʼянства.\r\nПрысутнічае на усіх бяседах, сочыць за тым, хто колькі выпівае, падбухторвае не надта пітушчых выпіць паболей. Калі яму гэта не ўдаецца, падсыпае ў чарку нейкага зелля, пасля чаго чалавек робіцца зусім пʼяны. Апівень любіць забаўляцца з пʼянымі, дражніць іх і скідвае пад стол. Пабачыць апіўня можна толькі на добрым падпітку.\r\nЯго выгляд спалучае ў сабе антрапа - зааморфныя рысы. Ен з‘яўляецца невялічкай істотаю, парослай цёмнай, рэдкай поўсцю. Галава па форме нагадвала чалавечую, толькі са свіным рылам, а там, дзе павінны быць бровы — тырчаць маленькія, як у маладога бычка, рожкі. Ёсць у апіўня  хвосцік, закручаны, нібы ў парсючка, ножкі з капытцамі. Ходзіць проста або ракам.\r\nСцвярджаюць, што няма на свеце п'яніцы, які б здолелі перапіць апіўня. Сам жа нячысцік, колькі б ні выпіў, надта пʼяным не будзе. Пра чалавека, які шмат п'е, але не п'янее, гавораць, што ён п'е, як апівень.\r\n"
                , "apivien.png",
                new List<SpiritsClassification> { SpiritsClassification.home },
                new List<string> { "Мядзельскі раён" }));

            Spirits.Add(new Spirit(24.667601, 53.685770,
                "Лесавік",
                "У беларускай міфалогіі дух - увасабленне лесу як часткі прасторы, патэнцыйна чужой чалавеку. Кожны лясны масіў мае сваяго гаспадара - Лесавіка, які апякуецца ўсімі звярамі і птушкамі(ратуе іх ад пажару, паляўнічых і г.д.)"
                , "liesavik.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Гродзенская вобласць" }));


            Spirits.Add(new Spirit(28.223098, 55.952912,
                "Лізун",
                "Міфічны персанаж, якім палохалі дзяцей на Віцебшчыне. Калі малы надта плакаў, яму казалі: «Ціша, дзетка, а то цябе лізун зліжыць»."
                , "lizun.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Віцебская вобласць" }));

            Spirits.Add(new Spirit(26.887915, 54.882007,
                "Бадзюля",
                "БАДЗЮЛЯ, дух, які змушае чалавека валацужнічаць, гоніць з наседжанага месца. Бадзюля туляецца каля дарог.\r\nБліжай да зімы увязваецца следам за чалавекам, у хаце якога і пасяляецца. З гэтага часу там назіраюцца усялякія прыкрасці: чалавек пачынае піць, цускаючы на вецер свой набытак. \r\nБадзюля чакае, калі той усе растрасе і\r\nпойдзе па свеце. Каб прагнаць бадзюлю трэба чыста вымесці і вымыць у хаце, і бруд выліць наводлег на заход сонца.\r\nТады, кажуць, можна убачыць, як Бадзюля ўцякае, з перапуду забываючы схаваць сваё аблічча. Мае выгляд немаладой жанчыны з валікімі на брух грудзьмі. На ёй няма ніякага адзення, толькі брудная падраная посцілка, якая амаль не прыкрывае паропанага струплівага цела. Твар у яе непрыгожы: цукатыя вочы, кароткі шырокі нос, тоўстыя адвіслыя вусны, валасы, збітыя у каўтун.\r\n"
                , "badziulia.png",
                new List<SpiritsClassification> { SpiritsClassification.home },
                new List<string> { "Мядзельскі раён" }));

            Spirits.Add(new Spirit(26.881330, 54.510453,
                "Баламуцень",
                "БАЛАМУЦЕНЬ, вадзяны чалавек.\r\nВа ўяуленнях маляваўся надзвычай\r\nнепрыгожым: галава его нагадвала\r\nзбан, твар такі азызлы, што амаль не відаць вачэй, гусіная скура, кароценькія, тоненькія і крывенькія ножкі, вялізны жывот. Баламуцень - халасцяк, пры гэтым вельмі любіць жанчын. Калі прыходзіць пара кахання, ён набліжаецца да тых месцаў, дзе жанчыны купаюцца ці палошчуць бялізну.\r\nБаламуцень жартуе з імі, скідвае з кладак, муціць ваду і выглядвае сярод іх сабе жанчыну. Бялізну менавіта той, якую ён упадабаў, Баламуцень заганяе да другога\r\nберага і, падчапіўшы за карчы, чакае. Пры набліжанні жанчыны ён паўстае перад ёю ва ўсім сваім харастве і напускае на абранніцу чары, і тая паслухмяна ідзе за ім. Баламуцень ніколі не бярэ жанчын назаусёды, ён адпускае іх дадому, і яны набываюць здольнасць не тануць нават тады, калі б гэтага захацелі.\r\n"
                , "balamucien.png",
                new List<SpiritsClassification> { SpiritsClassification.water },
                new List<string> { "Вілейскі раён" }));

            Spirits.Add(new Spirit(26.977118, 54.519014,
                "Вужалкі",
                "Вужалкі – дочкі Вужынага Караля. Яны не носяць адзен­ня, але вельмі любяць гожыя каралі ды бранзалеты. Калі чалавек знойдзе ўпрыгожанне, згубленае Вужалкай, яму пашчасціць у жыцці, а яшчэ ён можа не баяцца змяіных укусаў.\r\nВужалкі не варожыя людзям, але вакол іх заўсёды шмат змей – так Вужыны Кароль ахоўвае сваіх дачок. Крыўдзіць Вужалку не варта, бо крыўдніка чакае праклён ды розныя няшчасці.\r\nВужалкі вельмі палахлівыя. Заўважыўшы падарожніка, Вужалка са­ скочыць з дрэва, перакінецца ў невялічкую змейку і стане непрыкмет­най сярод іншых паўзуноў.\r\n"
                , "vuzhalki.png",
                new List<SpiritsClassification> { SpiritsClassification.forest, SpiritsClassification.water },
                new List<string> { "Вілейшчына" }));


            Spirits.Add(new Spirit(27.260015, 54.128937,
                "Лясны цмок",
                "Цмокі бываюць розныя. І паводле месца жыхарства: лясныя, вадзя­ныя, хатнія. І памерам: хто ростам з котку, а хто – з гару. І паводзінамі: хто шкодзіць, а хто, наадварот, дапамагае.\r\n"
                , "liasny_cmok.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Міншчына" }));

            Spirits.Add(new Spirit(26.631904, 55.392669,
                "Нактыр",
                "Вупыр, нябожчык, які па начах вылазіць з магілы ды ходзіць да сваіх жывых сваякоў, турбуе іх, шкодзіць хатняй жывёле, нават п’е іх кроў. Непрытомнік наведвае менавіта сваіх сваякоў.\r\nБеларусы верылі, што ў Накты'раў пасля смерці ператвараліся чарадзеі або вядзьмаркі. Каб Непрытомнік не хадзіў пасля смерці, то пад час пахавання чараўніка, усё рабілі наадварот: памерлага выносілі галавой на перад, на скрыжаванні дарог труну пераварочвалі кругам. Сэнс такіх дзеянняў заблыдаць пачвару, каб яна не знайшла дарогу дадому.\r\n"
                , "naktyr.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Відзы" }));

            Spirits.Add(new Spirit(30.071805, 55.357742,
                "Капялюшнік",
                "Капялюшнікі не варожыя да людзей. Яны могуць выпадкова напа­лохаць, чалавек можа заблукаць праз іх агеньчыкі. Але яны не жадаюць людзям ліха. Наадварот, людзі ім цікавыя. Аднак Капялюшнікі такія са­рамлівыя, што заўсёды бянтэжацца і праз гэта не могуць наблізіцца ды пачаць размову.\r\n"
                , "kapialiushnik.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Віцебшчына" }));

            Spirits.Add(new Spirit(23.859640, 52.612107,
                "Пушчавік",
                "Пушчавік уладарыць у некранутай частцы лесу, так званай “пушчы­ дрымушчы”. Ростам ён вышэйшы за самае высокае дрэва ў сваіх уладан­ нях, увесь зарослы доўгім мохам.\r\nЛюдзі для яго – ворагі, бо яны высякаюць лясы. Ён не шкадуе нікога: ні дзяцей, ні жанчын, ні нават свойскай жывёлы, якая служыць чалавеку. На шчасце, праз свой рост Пушчавік непаваротны, няўклюдны і дрэнна бачыць, што адбываецца ў яго пад нагамі.\r\nСустрэць Пушчавіка няпроста. Ён жыве ў самым сэрцы пушчы, ато­ чаным непраходным лесам ды балотамі, якія не замярзаюць нават у са­ мыя суворыя зімы.\r\n"
                , "pushczavik.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Пучшы, Белавежская пушча" }));

            Spirits.Add(new Spirit(26.866598, 53.989342,
                "Лесавая",
                "Лесавая, Лешачыха – гэта жонка Лесуна. У лесе яе сустрэнеш рэдка, звычайна яна ў гушчары даглядае жытло ды гадуе дзяцей. Часам яна выходзіць у вёскі ды падмяняе людскіх дзяцей на сваіх, крыклівых ды непаслухмяных. Такія дзеці заўсёды ўцякаюць потым у лес ды робяцца Лесунамі.\r\n"
                , "liesavaja.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Мінская вобласць, веска Сульжычы" }));

            Spirits.Add(new Spirit(23.830095, 52.682710,
                "Паднор",
                "Мышы­ны кароль, Паднор. Ён гаспадарыць не толькі над ляснымі мышамі, але і над палявымі, і над хатнімі. Ён ахоўвае гры­ зуноў ад драпежнікаў ды голаду.\r\nПра тое, як выглядае Паднор, вядо­ма няшмат. Існуе такое апісанне: процьма мышэй, спле­ценых між сабой хвастамі. Яны ўвесь час варушацца, пішчаць, прынюхваюц­ца. Наверсе ўзвышаецца мыш, буйней­шая за астатніх. На галаве ў яе прыродная карона з жоўтых костак.\r\nДа людзей Паднор ставіцца абыякава.\r\nКажуць, часам дзеля забавы можа пагутарыць па­чалавечы.\r\n"
                , "padnor.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "Лясы, пушчы" }));


            Spirits.Add(new Spirit(26.973935, 53.318083,
                "Пуннік",
                "Сядзібны нячысцік, што жыве ў пуні. Ён уяўляўся чалавекападобнай істотай. Матэрыялізаванае аблічча Пуннік мае толькі калі ў пунях ёсць сена. Загінуць ён можа ад грому і маланкі, тады ён ператвараецца ў пыл."
                , "punnik.png",
                new List<SpiritsClassification> { SpiritsClassification.home },
                new List<string> { "Мінская вобласць" }));

            Spirits.Add(new Spirit(26.838939, 54.307332,
                "Пячурнік",
                "Дух, які жыве ў печы ці за печчу. Абліччам нагадвае ката, толькі ходзіць на задніх лапах, чалавеку стараецца не паказвацца. З’яўляўся, як і дамавік, апекуном таго дома, дзе жыў. У знак пашаны гаспадыня выстаўляла ноччу на печы пачастунак пячурніку — рэшткі вячэры альбо малако."
                , "piaczurnik.png",
                new List<SpiritsClassification> { SpiritsClassification.home },
                new List<string> { "Маладзечаншчына" }));

            Spirits.Add(new Spirit(25.916513, 53.433522,
                "Свіцязянкі",
                "Паводле падання, у Свіцязянак ператварыліся жанчыны горада Свіцязь, затопленага багамі па іх просьбе вадой, каб пазбегнуць зганьбавання варожым войскам. Сцвярджалі, што часта можна было чуць у возеры іх стогны. Уяўлялі іх у выглядзе белатварых дзяўчын з даўгімі распушчанымі валасамі. Злоўленая ў невад Свіцязянка апавядала сумную гісторыю свайго горада.\r\n"
                , "sviciazianki.png",
                new List<SpiritsClassification> { SpiritsClassification.water },
                new List<string> { "В.Свіцязь, Навагрудчына" }));

            Spirits.Add(new Spirit(25.235527, 52.909148,
                "Стрыга",
                "Міфічная істота, чараўніца, якая турбуе цяжарных і падмяняе маткам дзяцей, падкідваючы ім замест добрага і тоўстага худое і плаксівае. Уяўлялася худой бледнай жанчынаю высокага росту з распушчанымі валасамі, з запалымі шчокамі і зялёнымі кашачымі вачыма."
                , "stryha.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Гродзеншчына" }));

            Spirits.Add(new Spirit(27.031618, 55.639491,
                "Сярбай",
                "Міфічная істота, якая ўвасабляла голад. Каб прагнаць сярбая, трэба было першы сжаты сноп прынесці ў хату. Сярбай згадваўся на Браслаўшчыне."
                , "siarbaj.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Браслаўшчына" }));

            Spirits.Add(new Spirit(26.915053, 54.487892,
                "Хапун",
                "Старэнькі, невялічкі дзядок, з доўгай барадой і торбаю. Ён летаў над зямлёю і хапаў непаслухмяных дзяцей, асабліва тых, што не слухалі бацькоў ці адыходзілі далёка ад дому. Хапун саджаў дзіця ў торбу. Дзіцячы крык з той завязанай торбы ніхто не мог пачуць, і Хапуна ніхто не мог дагнаць, бо лётаў ён вельмі хутка. Прычым дзіця не магло нават схавацца і ўцячы, бо Хапун нябачны, і пабачыць яго можна было толькі тады, калі ён развяжа торбу. А рабіў ён гэта толькі ў сваім жытле.\r\n"
                , "chapun.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Вілейшчына" }));

            Spirits.Add(new Spirit(26.927728, 54.871088,
                "Хіхітун",
                "Маленькі нячысцік, які ўвесь час жыве за спіной у чалавека. Калі з блізкімі яму людзьмі здараецца нешта брыдкае, Хіхітун радуецца і тоненька смяецца: \"Хі-хі, хі-хі, хі-хі\". Кажуць, што, калі рэзка абарнуцца ў гэты момант, можна пабачыць Хіхітуна. Гэта маленькае стварэнне, якое нагадвае малпу, але на макаўцы ў яго невялічкія рожкі.\r\n"
                , "chichitun.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Мядзельшчына" }));

            Spirits.Add(new Spirit(30.118391, 54.471498,
                "Кука",
                "Кука - нячысцік, якім у Беларусі палохаюць дзяцей, якія адмаўляюцца класціся спаць\r\nноччу. Кука мае жаночую прыроду, жыве ў\r\nцемры і сама чорнага колеру. Мала каму ўдалося разгледзець дэталі яе знешнасці, але звычайна людзі адзначаюць такія рысы як крывізна, сагнутасць, вострагаловасць.\r\n"
                , "kuka.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Віцебская вобласць, веска Дзятлава" }));

            Spirits.Add(new Spirit(31.316510, 52.421101,
                "Дзюндзік",
                "Дзюндзік не зʼяуляецца смяротна небяспечнай істотай, але вельмі не любіць дзяцей і пры сустрэчы можа добра збіць малога сваёй даўбежкай. Звычайна, нячысцік выглядае, як клубок бруднай збітай травы з доўгімі нагамі, падобнымі на бусліныя. І заўсёды ў руках жалезная даўбёжка - нешта накшталт малатка,\r\nчасам нячысцік вырабляе яго сабе сам, часам любіць зкрасці прыладу ў чалавека.\r\nТыя што жывуць у жыце больш жоўтыя, тыя што\r\nсустракаюцца на агародах больш зялёныя.\r\n"
                , "dziundzik.png",
                new List<SpiritsClassification> { SpiritsClassification.field },
                new List<string> { "Добруш, Гомельская вобласць" }));

            Spirits.Add(new Spirit(30.432209, 53.236777,
                "Паралікі",
                "Паралікі - у беларускай міфалогії гэта ліхія духі, якія ў адрозненне ад іншых нячысцікаў апаноўваюць ахвяру не адразу, а падступаюцца да яе спадцішка, часам напрацягу некалькіх гадоў. Часцей іх ахвярамі становяцца людзі сталага узросту. Дзейнічаюць паралікі гуртам, працуючы папераменна, а вось пры супраціуленні ахвяры - ужо разам.\r\nЧалавек пазнае прысутнасць Паралікоў толькі тады, калі яны зжыліся з ім, і адпрэчыць іх ўжо\r\nняма ніякай магчымасці. Выглядаюць гэтыя нячысцікі жудасна: кайматыя, касыя, з рознымі рукамі і чортавымі нагамі. Даймаюць галоўным чынам цела чалавека, а з ім їх ахвяраю становіцца іх душа.\r\n"
                , "paraliki.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "гарадок Званец" }));

            Spirits.Add(new Spirit(27.740578, 52.066329,
                "Лямец",
                "Ламец - у беларускім фальклоры злы балотны дэман. Гэта ліхі дух, вера ў якога была на Палессі, згадваецца ў выслоỹях і праклёнах на Тураўшчыне.\r\nЛамец бязлітасны да простых людзей. Яго магічныя абдымкі зачароўваюць, пасля чаго дэман душыць чалавека, ламае яму ўсе косці. Але пры гэтым, Ламец абыходзіць бокам валодання чарцей, ён не звязваецца з тымі, хто ім прыслугоўвае - ведзмамі ды ведзмакамі.\r\nКажуць Хапун хапае, Кадук кідае (маецца на вазе прыступ эпілепсії), Лізун зліжа, а Ламец\r\nзламае…\r\n"
                , "liamiec.png",
                new List<SpiritsClassification> { SpiritsClassification.water },
                new List<string> { "Палессе, Тураўшчына" }));

            Spirits.Add(new Spirit(27.640362, 52.083516,
            "Лямец",
            "Ламец - у беларускім фальклоры злы балотны дэман. Гэта ліхі дух, вера ў якога была на Палессі, згадваецца ў выслоỹях і праклёнах на Тураўшчыне.\r\nЛамец бязлітасны да простых людзей. Яго магічныя абдымкі зачароўваюць, пасля чаго дэман душыць чалавека, ламае яму ўсе косці. Але пры гэтым, Ламец абыходзіць бокам валодання чарцей, ён не звязваецца з тымі, хто ім прыслугоўвае - ведзмамі ды ведзмакамі.\r\nКажуць Хапун хапае, Кадук кідае (маецца на вазе прыступ эпілепсії), Лізун зліжа, а Ламец\r\nзламае…\r\n"
            , "liamiec.png",
            new List<SpiritsClassification> { SpiritsClassification.water },
            new List<string> { "Палессе, Тураўшчына" }));

            Spirits.Add(new Spirit(25.221720, 54.022537,
                "Курнэля",
                "Курнэля і сапрауды падобная знешне на сароку, але мае хвост лісы. Толькі ў адрозненні ад лісінага, гэты як палена цвёрды і цяжкі. Гэтыя нячысцікі часам зʼяуляюцца ноччу ў дварах і наводзяць шляхетны шум. Пакуль яны пануюць,\r\nна вуліцу лепш не выходзіць.\r\n"
                , "kurnalia.png",
                new List<SpiritsClassification> { SpiritsClassification.forest },
                new List<string> { "в.Жырмуны , Гродзенская вобласць" }));

            Spirits.Add(new Spirit(27.685843, 55.138799,
                "Галышка",
                "Галышка - страшная русалка з лысай галавой,\r\nбелай скурай і чырвонымі вачыма. Яе грудзі пакрытыя іголкамі, яна ловіць дзяцей і падлеткаў у палях, прымушае піць малако з калючых грудзей. Верагодна, Галышка гэта продак Цыцохі і Цыгры. Калісьці даўно гэтыя русалкі мігравалі на Палессе і зʼявіліся такія жабападобныя разнавіднасці, якія па сваіх паводзінах вельмі\r\nпадобныя.\r\n"
                , "halyshka.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Глыбокае, Віцебская вобласць" }));

            Spirits.Add(new Spirit(31.172493, 52.559170,
                "Спорнік",
                "Спорнік або Спарнік - у беларускім фальклоры адзін з сядзібных духаў, захавальнік і памнажальнік багацця гаспадара.\r\nСпорнік звычайна набывае постаць чорнага ката і ў гэтым выглядзе яго можна пабачыць у свіране\r\nабо гумне, дзе гэты кот на таку адрыгвае зерня.\r\nКалі недасведчаны парабак вырашыць прагнаць такога ката, кінуць у яго што або нават ударыць, то Спорнік спаліць усе гумно, а сам перавернецца ў дугу.\r\nУ той жа час, калі хто здагадаецца перакінуць такую дугу сабе ў двор, то Спорнік будзе жыць у яго і ўсяляк узбагачаць.\r\nЛюбімая страва Спорніка гэта несаленая яешня.\r\n"
                , "spornik.png",
                new List<SpiritsClassification> { SpiritsClassification.home },
                new List<string> { "Ветка, Гомельская вобласць\r\n" }));

            Spirits.Add(new Spirit(30.096356, 54.758875,
                "Каўтун",
                "\"Каўтун, Калтун, Госцец - у\r\nбеларускім фальклоры гэта злы дух, дэман, персаніфікацыя аднаіменнай хваробы. Знешне выглядала усё так, што у чалавека былі збітыя, заблытаныя ў адзін ком валасы, быццам бы поўсць. Маглі быць і больш цяжкія сімптомы: боль, ламаццё ў касцях, сутаргі, скураныя язвы, высып, псіхічныя расстройствы. Каўтун мог трапіць у чалавека праз Сурокі, разам са злым ветрам - Падвей, або нават з кажаном, які мог трапіць у валасы і заблытацца ў іх.\r\nЛюдзі не спяшаліся пазбавіцца ад Каўтуна, часам лічылася, што гэта добры знак і Каўтун бароніць чалавека ад іншага зла, ад Чарцей. Кайтун на хатняй жывёле прыпісваўся дзеянням хатніх або дваравых духай - Хлеўнік або Хатнік, і таму здымаць яго было забаронена, каб ненашкодзіць жывеле.\r\nКаўтун мог вырастаць да вялікіх памераў і чалавечыя пакуты станавіліся невыноснымі.\r\nДэман мог перамяшчацца па ўсім целе чалавека.\r\nПазбаўляліся ад Катуна рознымі шляхамі: замовамі, перабівалі кавалдай або малатком на камені, перапальвалі срэбным дротам. Нельга было выкарыстоўваць нож або нажніцы, у такіх выпадках можна было пабачыць кроў дэмана і ён мог перакінуцца на таго, хто яго зрэзаў.\r\nПотым Кайтун зпальвалі або закопвалі ў белай тканіне ў зямлю, каб ён не вяртаўся болей. \"\r\n"
                , "kautun.png",
                new List<SpiritsClassification> { SpiritsClassification.dark },
                new List<string> { "Янава, Віцебская вобласць \r\n" }));

        }


        private async Task CreateSpiritMarkers()
        {
            var SpiritsGraphicOverlay = new GraphicsOverlay();
            GraphicsOverlayCollection overlays = MainMapView.GraphicsOverlays;
            overlays.Add(SpiritsGraphicOverlay);

            MainMapView.GraphicsOverlays = overlays;

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            foreach (var spirit in Spirits)
            {
                
                Stream resourceStream = currentAssembly.GetManifestResourceStream(
                    "SpiritsFirstTry.Resources.Images." + spirit.Image_name);
                PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(resourceStream);
                spirit.markerSymbol = pinSymbol;
                pinSymbol.Width = 40;
                pinSymbol.Height = 40;

                Graphic pinGraphic = new Graphic(spirit.mapPoint, pinSymbol);
                spirit.pinGraphic = pinGraphic;
                Spirit.maxzindex = Math.Max(pinGraphic.ZIndex, Spirit.maxzindex);

                SpiritsGraphicOverlay.Graphics.Add(pinGraphic);
            }
        }
    }
}
