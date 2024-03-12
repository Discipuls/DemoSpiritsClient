using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsFirstTry.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        RestService _restService;

        public MapView MainMapView { get; set; }
        public static List<Spirit> Spirits { get; set; }
        BottomSheetView bottomSheeet;
        BottomSheetViewModel bottomSheeetVm;
        public GraphicsOverlay polygonOverlay;
        public static List<Graphic> regions = new List<Graphic>();

        public MainViewModel(RestService restService) {
            LoadRegions();
            LoadSpirits();

            _restService = restService;

            bottomSheeetVm = new BottomSheetViewModel();
            bottomSheeetVm.spiritList = Spirits;
            bottomSheeetVm.regions = regions;
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
            foreach (var spirit in Spirits)
            {
                spirit.pinGraphic.IsVisible = true;
            }
            if (min_dist / MainMapView.MapScale * 100 < 1)
            {
                bottomSheeetVm.IsSpiritOpend = true;
                bottomSheeetVm.IsSearchOpend = false;



                foreach(var spirit in Spirits)
                {
                    spirit.markerSymbol.Height = 40;
                    spirit.markerSymbol.Width = 40;
                    spirit.polygonGraphic.IsVisible = false;
                }
                closest.polygonGraphic.IsVisible = true;
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
                foreach (var spirit in Spirits)
                {
                    spirit.markerSymbol.Height = 40;
                    spirit.markerSymbol.Width = 40;
                    spirit.polygonGraphic.IsVisible = false;
                }
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

        public void LoadRegions()
        {
            List<MapPoint> polygonPoints1 = new List<MapPoint>
            {new MapPoint(26.2005218,55.0035572, SpatialReferences.Wgs84),new MapPoint(26.0576995,54.9499634, SpatialReferences.Wgs84),new MapPoint(25.8874114,54.934187, SpatialReferences.Wgs84),new MapPoint(25.7006438,54.8045861, SpatialReferences.Wgs84),new MapPoint(25.739096,54.5218216, SpatialReferences.Wgs84),new MapPoint(25.5907805,54.4100823, SpatialReferences.Wgs84),new MapPoint(25.5468352,54.3044485, SpatialReferences.Wgs84),new MapPoint(25.7336028,54.2755921, SpatialReferences.Wgs84),new MapPoint(25.7775481,54.2113944, SpatialReferences.Wgs84),new MapPoint(25.6566985,54.127788, SpatialReferences.Wgs84),new MapPoint(25.5028899,54.1760431, SpatialReferences.Wgs84),new MapPoint(25.5907805,54.2563433, SpatialReferences.Wgs84),new MapPoint(25.4644378,54.2884197, SpatialReferences.Wgs84),new MapPoint(25.2282317,54.249925, SpatialReferences.Wgs84),new MapPoint(25.1643192,54.1959284, SpatialReferences.Wgs84),new MapPoint(25.0956547,54.144479, SpatialReferences.Wgs84),new MapPoint(24.9775517,54.1460878, SpatialReferences.Wgs84),new MapPoint(24.9628808,54.1698986, SpatialReferences.Wgs84),new MapPoint(24.9120691,54.1602507, SpatialReferences.Wgs84),new MapPoint(24.8379114,54.1449701, SpatialReferences.Wgs84),new MapPoint(24.7706201,54.1107696, SpatialReferences.Wgs84),new MapPoint(24.8454645,54.0285731, SpatialReferences.Wgs84),new MapPoint(24.7026422,53.9631834, SpatialReferences.Wgs84),new MapPoint(24.6930292,54.0164716, SpatialReferences.Wgs84),new MapPoint(24.513517,53.9572525, SpatialReferences.Wgs84),new MapPoint(24.4448525,53.8925601, SpatialReferences.Wgs84),new MapPoint(24.1894203,53.9524041, SpatialReferences.Wgs84),new MapPoint(23.9422279,53.9184491, SpatialReferences.Wgs84),new MapPoint(23.9120155,53.965332, SpatialReferences.Wgs84),new MapPoint(23.7994057,53.8974155, SpatialReferences.Wgs84),new MapPoint(23.6626759,53.9349089, SpatialReferences.Wgs84),new MapPoint(23.5143605,53.9478423, SpatialReferences.Wgs84),new MapPoint(23.5473195,53.7729031, SpatialReferences.Wgs84),new MapPoint(23.6352101,53.5613569, SpatialReferences.Wgs84),new MapPoint(23.8384572,53.2502609, SpatialReferences.Wgs84),new MapPoint(23.8902083,53.0736536, SpatialReferences.Wgs84),new MapPoint(23.9259138,52.8619208, SpatialReferences.Wgs84),new MapPoint(23.9478865,52.7756079, SpatialReferences.Wgs84),new MapPoint(24.1345506,52.805573, SpatialReferences.Wgs84),new MapPoint(24.2348009,52.7582268, SpatialReferences.Wgs84),new MapPoint(24.3885904,52.771795, SpatialReferences.Wgs84),new MapPoint(24.50944,52.7485256, SpatialReferences.Wgs84),new MapPoint(24.5808512,52.7626549, SpatialReferences.Wgs84),new MapPoint(24.5712381,52.8771825, SpatialReferences.Wgs84),new MapPoint(24.6591287,52.9111506, SpatialReferences.Wgs84),new MapPoint(24.641276,52.9517118, SpatialReferences.Wgs84),new MapPoint(24.7236734,52.9748725, SpatialReferences.Wgs84),new MapPoint(24.7621256,52.941782, SpatialReferences.Wgs84),new MapPoint(24.877482,52.9566758, SpatialReferences.Wgs84),new MapPoint(25.0010782,52.9268829, SpatialReferences.Wgs84),new MapPoint(25.0312906,52.8904415, SpatialReferences.Wgs84),new MapPoint(25.0642496,52.9277108, SpatialReferences.Wgs84),new MapPoint(25.0697428,52.8340626, SpatialReferences.Wgs84),new MapPoint(25.1507669,52.8589447, SpatialReferences.Wgs84),new MapPoint(25.2084451,52.8440171, SpatialReferences.Wgs84),new MapPoint(25.235911,52.8771825, SpatialReferences.Wgs84),new MapPoint(25.278483,52.8813263, SpatialReferences.Wgs84),new MapPoint(25.3334146,52.8315736, SpatialReferences.Wgs84),new MapPoint(25.363627,52.8647484, SpatialReferences.Wgs84),new MapPoint(25.3196817,52.8746959, SpatialReferences.Wgs84),new MapPoint(25.3169351,52.8937556, SpatialReferences.Wgs84),new MapPoint(25.3498941,52.9210875, SpatialReferences.Wgs84),new MapPoint(25.3169351,52.9376438, SpatialReferences.Wgs84),new MapPoint(25.4089456,52.9450922, SpatialReferences.Wgs84),new MapPoint(25.4597574,53.0071115, SpatialReferences.Wgs84),new MapPoint(25.5325418,53.0236349, SpatialReferences.Wgs84),new MapPoint(25.5929666,53.0607896, SpatialReferences.Wgs84),new MapPoint(25.6010904,53.1684804, SpatialReferences.Wgs84),new MapPoint(25.6230631,53.3082028, SpatialReferences.Wgs84),new MapPoint(25.7109537,53.3852654, SpatialReferences.Wgs84),new MapPoint(25.9032144,53.4032812, SpatialReferences.Wgs84),new MapPoint(26.1970987,53.3656031, SpatialReferences.Wgs84),new MapPoint(26.3701334,53.3574078, SpatialReferences.Wgs84),new MapPoint(26.5404215,53.3934554, SpatialReferences.Wgs84),new MapPoint(26.5733804,53.4883437, SpatialReferences.Wgs84),new MapPoint(26.4662637,53.6107301, SpatialReferences.Wgs84),new MapPoint(26.2959757,53.7165121, SpatialReferences.Wgs84),new MapPoint(26.2311377,53.8641505, SpatialReferences.Wgs84),new MapPoint(26.364347,53.8941035, SpatialReferences.Wgs84),new MapPoint(26.37396,53.9280783, SpatialReferences.Wgs84),new MapPoint(26.4247718,53.9547534, SpatialReferences.Wgs84),new MapPoint(26.4014258,54.0137013, SpatialReferences.Wgs84),new MapPoint(26.3258948,53.985449, SpatialReferences.Wgs84),new MapPoint(26.2943091,53.9676806, SpatialReferences.Wgs84),new MapPoint(26.3286414,54.0548346, SpatialReferences.Wgs84),new MapPoint(26.2187781,54.0338698, SpatialReferences.Wgs84),new MapPoint(26.1652198,54.0185427, SpatialReferences.Wgs84),new MapPoint(26.1336341,54.0951219, SpatialReferences.Wgs84),new MapPoint(26.1336341,54.1305424, SpatialReferences.Wgs84),new MapPoint(26.1363807,54.1611084, SpatialReferences.Wgs84),new MapPoint(26.0745826,54.1410017, SpatialReferences.Wgs84),new MapPoint(26.1089148,54.1055902, SpatialReferences.Wgs84),new MapPoint(26.0196509,54.1353701, SpatialReferences.Wgs84),new MapPoint(26.0181769,54.1592728, SpatialReferences.Wgs84),new MapPoint(26.0580023,54.21231, SpatialReferences.Wgs84),new MapPoint(26.0786017,54.1821836, SpatialReferences.Wgs84),new MapPoint(26.1026343,54.1845945, SpatialReferences.Wgs84),new MapPoint(26.1795386,54.2251572, SpatialReferences.Wgs84),new MapPoint(26.2433966,54.2255586, SpatialReferences.Wgs84),new MapPoint(26.306568,54.2030735, SpatialReferences.Wgs84),new MapPoint(26.3477667,54.21231, SpatialReferences.Wgs84),new MapPoint(26.4269197,54.2799514, SpatialReferences.Wgs84),new MapPoint(26.494211,54.3152138, SpatialReferences.Wgs84),new MapPoint(26.5257967,54.3736508, SpatialReferences.Wgs84),new MapPoint(26.6150606,54.421619, SpatialReferences.Wgs84),new MapPoint(26.6535127,54.5070232, SpatialReferences.Wgs84),new MapPoint(26.6601572,54.5951466, SpatialReferences.Wgs84),new MapPoint(26.6903696,54.6508017, SpatialReferences.Wgs84),new MapPoint(26.5983591,54.6452396, SpatialReferences.Wgs84),new MapPoint(26.5406809,54.7166941, SpatialReferences.Wgs84),new MapPoint(26.4549323,54.8217607, SpatialReferences.Wgs84),new MapPoint(26.4192268,54.8320448, SpatialReferences.Wgs84),new MapPoint(26.368415,54.8225519, SpatialReferences.Wgs84),new MapPoint(26.3450691,54.8533956, SpatialReferences.Wgs84),new MapPoint(26.3653523,54.8851165, SpatialReferences.Wgs84),new MapPoint(26.3083607,54.923805, SpatialReferences.Wgs84),new MapPoint(26.2630421,54.9470791, SpatialReferences.Wgs84),new MapPoint(26.243816,54.9857081, SpatialReferences.Wgs84),new MapPoint(26.212917,55.0002829, SpatialReferences.Wgs84),new MapPoint(26.2005218,55.0035572, SpatialReferences.Wgs84)
            };
            var mahouRivieraPolygon1 = new Polygon(polygonPoints1);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline1 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
            var polygonFillSymbol1 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline1);


            var polygonGraphic1 = new Graphic(mahouRivieraPolygon1, polygonFillSymbol1);
            polygonGraphic1.IsVisible = false;
            regions.Add(polygonGraphic1);



             var polygonPoints2 = new List<MapPoint>
            {
                new MapPoint(26.2005218,55.0035572, SpatialReferences.Wgs84),new MapPoint(26.2368071,55.0480161, SpatialReferences.Wgs84),new MapPoint(26.2601531,55.0755427, SpatialReferences.Wgs84),new MapPoint(26.2230742,55.0991219, SpatialReferences.Wgs84),new MapPoint(26.2532866,55.1187605, SpatialReferences.Wgs84),new MapPoint(26.3123381,55.1226871, SpatialReferences.Wgs84),new MapPoint(26.3240111,55.1403519, SpatialReferences.Wgs84),new MapPoint(26.3432372,55.1493776, SpatialReferences.Wgs84),new MapPoint(26.3926756,55.1415293, SpatialReferences.Wgs84),new MapPoint(26.4304411,55.1403519, SpatialReferences.Wgs84),new MapPoint(26.4640868,55.1305391, SpatialReferences.Wgs84),new MapPoint(26.4826262,55.1509471, SpatialReferences.Wgs84),new MapPoint(26.5203917,55.1580089, SpatialReferences.Wgs84),new MapPoint(26.6158354,55.1450612, SpatialReferences.Wgs84),new MapPoint(26.703726,55.1736975, SpatialReferences.Wgs84),new MapPoint(26.6796935,55.1940834, SpatialReferences.Wgs84),new MapPoint(26.7325652,55.2175926, SpatialReferences.Wgs84),new MapPoint(26.7243254,55.2461768, SpatialReferences.Wgs84),new MapPoint(26.8222311,55.2764905, SpatialReferences.Wgs84),new MapPoint(26.8270377,55.3097222, SpatialReferences.Wgs84),new MapPoint(26.7034415,55.3316008, SpatialReferences.Wgs84),new MapPoint(26.621044,55.325351, SpatialReferences.Wgs84),new MapPoint(26.5056876,55.3331631, SpatialReferences.Wgs84),new MapPoint(26.4515412,55.3397077, SpatialReferences.Wgs84),new MapPoint(26.4426148,55.3498594, SpatialReferences.Wgs84),new MapPoint(26.4855978,55.3736409, SpatialReferences.Wgs84),new MapPoint(26.5494558,55.3857343, SpatialReferences.Wgs84),new MapPoint(26.5583822,55.4145873, SpatialReferences.Wgs84),new MapPoint(26.5460226,55.446535, SpatialReferences.Wgs84),new MapPoint(26.5496978,55.4698743, SpatialReferences.Wgs84),new MapPoint(26.522232,55.5087769, SpatialReferences.Wgs84),new MapPoint(26.628932,55.5757434, SpatialReferences.Wgs84),new MapPoint(26.628932,55.6548506, SpatialReferences.Wgs84),new MapPoint(26.6591444,55.7059527, SpatialReferences.Wgs84),new MapPoint(26.727809,55.6989881, SpatialReferences.Wgs84),new MapPoint(26.7456618,55.6788614, SpatialReferences.Wgs84),new MapPoint(26.7827406,55.6788614, SpatialReferences.Wgs84),new MapPoint(26.8005934,55.6974403, SpatialReferences.Wgs84),new MapPoint(26.8733778,55.7129159, SpatialReferences.Wgs84),new MapPoint(26.8994704,55.7569879, SpatialReferences.Wgs84),new MapPoint(26.9200697,55.7917465, SpatialReferences.Wgs84),new MapPoint(26.9516554,55.7871138, SpatialReferences.Wgs84),new MapPoint(26.9681349,55.8095, SpatialReferences.Wgs84),new MapPoint(27.0230666,55.8272454, SpatialReferences.Wgs84),new MapPoint(27.0931044,55.8272454, SpatialReferences.Wgs84),new MapPoint(27.1082106,55.8457537, SpatialReferences.Wgs84),new MapPoint(27.1535292,55.8503794, SpatialReferences.Wgs84),new MapPoint(27.2015944,55.8295594, SpatialReferences.Wgs84),new MapPoint(27.2578993,55.7886581, SpatialReferences.Wgs84),new MapPoint(27.2924107,55.7867677, SpatialReferences.Wgs84),new MapPoint(27.3528355,55.802208, SpatialReferences.Wgs84),new MapPoint(27.3631352,55.816099, SpatialReferences.Wgs84),new MapPoint(27.6155684,55.7884333, SpatialReferences.Wgs84),new MapPoint(27.6375411,55.8963821, SpatialReferences.Wgs84),new MapPoint(27.8160689,56.0040313, SpatialReferences.Wgs84),new MapPoint(27.986357,56.1113815, SpatialReferences.Wgs84),new MapPoint(28.0654877,56.1391965, SpatialReferences.Wgs84),new MapPoint(28.1574982,56.169791, SpatialReferences.Wgs84),new MapPoint(28.2550019,56.1177659, SpatialReferences.Wgs84),new MapPoint(28.2398957,56.0817662, SpatialReferences.Wgs84),new MapPoint(28.3291596,56.05877, SpatialReferences.Wgs84),new MapPoint(28.3827179,56.1078119, SpatialReferences.Wgs84),new MapPoint(28.6038178,56.1062802, SpatialReferences.Wgs84),new MapPoint(28.6954935,56.0929364, SpatialReferences.Wgs84),new MapPoint(28.6282022,56.0722465, SpatialReferences.Wgs84),new MapPoint(28.6831339,56.0262291, SpatialReferences.Wgs84),new MapPoint(28.7202127,55.976315, SpatialReferences.Wgs84),new MapPoint(28.8506754,55.9509494, SpatialReferences.Wgs84),new MapPoint(28.9303262,55.9947524, SpatialReferences.Wgs84),new MapPoint(29.0676553,56.0339025, SpatialReferences.Wgs84),new MapPoint(29.2069455,55.9968747, SpatialReferences.Wgs84),new MapPoint(29.322302,55.9888099, SpatialReferences.Wgs84),new MapPoint(29.4807176,55.9328631, SpatialReferences.Wgs84),new MapPoint(29.392827,55.8558599, SpatialReferences.Wgs84),new MapPoint(29.3681078,55.7787038, SpatialReferences.Wgs84),new MapPoint(29.4202928,55.7354295, SpatialReferences.Wgs84),new MapPoint(29.5356493,55.6998468, SpatialReferences.Wgs84),new MapPoint(29.6070604,55.7663446, SpatialReferences.Wgs84),new MapPoint(29.6894579,55.7926032, SpatialReferences.Wgs84),new MapPoint(29.7718553,55.7787038, SpatialReferences.Wgs84),new MapPoint(29.8817186,55.8512348, SpatialReferences.Wgs84),new MapPoint(29.9833422,55.8743546, SpatialReferences.Wgs84),new MapPoint(30.0822191,55.8250158, SpatialReferences.Wgs84),new MapPoint(30.197786,55.8655186, SpatialReferences.Wgs84),new MapPoint(30.2884232,55.8709125, SpatialReferences.Wgs84),new MapPoint(30.2843033,55.8385381, SpatialReferences.Wgs84),new MapPoint(30.3639542,55.8099954, SpatialReferences.Wgs84),new MapPoint(30.4806839,55.8092237, SpatialReferences.Wgs84),new MapPoint(30.5191361,55.7752532, SpatialReferences.Wgs84),new MapPoint(30.6742571,55.6739649, SpatialReferences.Wgs84),new MapPoint(30.7621477,55.5886881, SpatialReferences.Wgs84),new MapPoint(30.8335588,55.6243719, SpatialReferences.Wgs84),new MapPoint(30.9269426,55.6119638, SpatialReferences.Wgs84),new MapPoint(30.9406755,55.5498645, SpatialReferences.Wgs84),new MapPoint(30.9209727,55.4695431, SpatialReferences.Wgs84),new MapPoint(30.9297544,55.3781025, SpatialReferences.Wgs84),new MapPoint(30.8198912,55.3265727, SpatialReferences.Wgs84),new MapPoint(30.8198912,55.2765404, SpatialReferences.Wgs84),new MapPoint(30.9036619,55.2515005, SpatialReferences.Wgs84),new MapPoint(31.0062341,55.1338326, SpatialReferences.Wgs84),new MapPoint(31.0172204,55.0395144, SpatialReferences.Wgs84),new MapPoint(30.9293298,55.0269219, SpatialReferences.Wgs84),new MapPoint(30.9513024,54.9654765, SpatialReferences.Wgs84),new MapPoint(30.8194665,54.9307734, SpatialReferences.Wgs84),new MapPoint(30.7562951,54.7916615, SpatialReferences.Wgs84),new MapPoint(30.9622887,54.7171663, SpatialReferences.Wgs84),new MapPoint(31.1023644,54.6647793, SpatialReferences.Wgs84),new MapPoint(31.1820153,54.6679562, SpatialReferences.Wgs84),new MapPoint(31.171029,54.5964158, SpatialReferences.Wgs84),new MapPoint(31.0858849,54.5088067, SpatialReferences.Wgs84),new MapPoint(31.1957482,54.4737103, SpatialReferences.Wgs84),new MapPoint(30.8428799,54.397573, SpatialReferences.Wgs84),new MapPoint(30.6231533,54.3767815, SpatialReferences.Wgs84),new MapPoint(30.5297695,54.258229, SpatialReferences.Wgs84),new MapPoint(30.3347622,54.3239557, SpatialReferences.Wgs84),new MapPoint(30.1013027,54.3623812, SpatialReferences.Wgs84),new MapPoint(29.9996792,54.2903039, SpatialReferences.Wgs84),new MapPoint(29.6838223,54.3223538, SpatialReferences.Wgs84),new MapPoint(29.5986782,54.3367682, SpatialReferences.Wgs84),new MapPoint(29.4778286,54.258229, SpatialReferences.Wgs84),new MapPoint(29.3322598,54.2341564, SpatialReferences.Wgs84),new MapPoint(29.4064175,54.3815805, SpatialReferences.Wgs84),new MapPoint(29.4695889,54.5921806, SpatialReferences.Wgs84),new MapPoint(29.4695889,54.6239952, SpatialReferences.Wgs84),new MapPoint(29.3350064,54.5889978, SpatialReferences.Wgs84),new MapPoint(29.2416226,54.5683034, SpatialReferences.Wgs84),new MapPoint(29.2169033,54.6319449, SpatialReferences.Wgs84),new MapPoint(29.1015469,54.6557849, SpatialReferences.Wgs84),new MapPoint(29.0301357,54.6128629, SpatialReferences.Wgs84),new MapPoint(28.9916836,54.57308, SpatialReferences.Wgs84),new MapPoint(28.9202725,54.5953632, SpatialReferences.Wgs84),new MapPoint(28.804916,54.5683034, SpatialReferences.Wgs84),new MapPoint(28.6648403,54.5810397, SpatialReferences.Wgs84),new MapPoint(28.6593472,54.6319449, SpatialReferences.Wgs84),new MapPoint(28.5769497,54.6748467, SpatialReferences.Wgs84),new MapPoint(28.4753262,54.6414826, SpatialReferences.Wgs84),new MapPoint(28.403915,54.5571559, SpatialReferences.Wgs84),new MapPoint(28.1939138,54.6814049, SpatialReferences.Wgs84),new MapPoint(28.0785573,54.6273817, SpatialReferences.Wgs84),new MapPoint(27.1933508,54.986487, SpatialReferences.Wgs84),new MapPoint(26.5679525,54.956625, SpatialReferences.Wgs84),new MapPoint(26.2005218,55.0035572, SpatialReferences.Wgs84)
            };
            var mahouRivieraPolygon2 = new Polygon(polygonPoints2);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline2 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
            var polygonFillSymbol2 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline2);


            var polygonGraphic2 = new Graphic(mahouRivieraPolygon2, polygonFillSymbol2);
            polygonGraphic2.IsVisible = false;
            regions.Add(polygonGraphic2);



            var polygonPoints3 = new List<MapPoint>
            {
                new MapPoint(23.9478865,52.7756079, SpatialReferences.Wgs84),new MapPoint(23.8255762,52.6556377, SpatialReferences.Wgs84),new MapPoint(23.5234522,52.5689153, SpatialReferences.Wgs84),new MapPoint(23.2872461,52.3916026, SpatialReferences.Wgs84),new MapPoint(23.1663965,52.2236709, SpatialReferences.Wgs84),new MapPoint(23.4410547,52.1799043, SpatialReferences.Wgs84),new MapPoint(23.6333155,52.0719874, SpatialReferences.Wgs84),new MapPoint(23.616836,51.8451891, SpatialReferences.Wgs84),new MapPoint(23.5619043,51.6820009, SpatialReferences.Wgs84),new MapPoint(23.6003565,51.4840274, SpatialReferences.Wgs84),new MapPoint(23.6717676,51.6547454, SpatialReferences.Wgs84),new MapPoint(23.9629053,51.5967732, SpatialReferences.Wgs84),new MapPoint(24.2485498,51.6956224, SpatialReferences.Wgs84),new MapPoint(24.3858789,51.8994542, SpatialReferences.Wgs84),new MapPoint(24.9791407,51.9096216, SpatialReferences.Wgs84),new MapPoint(25.2647852,51.9773456, SpatialReferences.Wgs84),new MapPoint(26.0338282,51.9333366, SpatialReferences.Wgs84),new MapPoint(26.4732813,51.8316127, SpatialReferences.Wgs84),new MapPoint(26.8468164,51.7670684, SpatialReferences.Wgs84),new MapPoint(27.2588037,51.7228532, SpatialReferences.Wgs84),new MapPoint(27.2862696,51.6138316, SpatialReferences.Wgs84),new MapPoint(27.5096404,51.6183642, SpatialReferences.Wgs84),new MapPoint(27.4821746,51.7239724, SpatialReferences.Wgs84),new MapPoint(27.5425994,51.7664867, SpatialReferences.Wgs84),new MapPoint(27.5508392,51.9835334, SpatialReferences.Wgs84),new MapPoint(27.597531,52.0308728, SpatialReferences.Wgs84),new MapPoint(27.5371062,52.1254014, SpatialReferences.Wgs84),new MapPoint(27.5592604,52.2218141, SpatialReferences.Wgs84),new MapPoint(27.4658766,52.289066, SpatialReferences.Wgs84),new MapPoint(27.3258009,52.3226538, SpatialReferences.Wgs84),new MapPoint(27.2653761,52.3260112, SpatialReferences.Wgs84),new MapPoint(27.2763624,52.3931051, SpatialReferences.Wgs84),new MapPoint(27.064726,52.507829, SpatialReferences.Wgs84),new MapPoint(27.0702192,52.6696864, SpatialReferences.Wgs84),new MapPoint(27.0152875,52.7695053, SpatialReferences.Wgs84),new MapPoint(26.8257734,52.7678435, SpatialReferences.Wgs84),new MapPoint(26.6664716,52.8475373, SpatialReferences.Wgs84),new MapPoint(26.5840741,52.8923009, SpatialReferences.Wgs84),new MapPoint(26.446745,52.8143493, SpatialReferences.Wgs84),new MapPoint(26.409826,52.9056353, SpatialReferences.Wgs84),new MapPoint(26.4180658,53.0032612, SpatialReferences.Wgs84),new MapPoint(26.527929,53.0709733, SpatialReferences.Wgs84),new MapPoint(26.3411615,53.1468163, SpatialReferences.Wgs84),new MapPoint(26.2687124,53.1948715, SpatialReferences.Wgs84),new MapPoint(26.3703359,53.2639253, SpatialReferences.Wgs84),new MapPoint(26.3923086,53.3295873, SpatialReferences.Wgs84),new MapPoint(26.3701334,53.3574078, SpatialReferences.Wgs84),new MapPoint(25.9032144,53.4032812, SpatialReferences.Wgs84),new MapPoint(25.7109537,53.3852654, SpatialReferences.Wgs84),new MapPoint(25.6124174,53.2981068, SpatialReferences.Wgs84),new MapPoint(25.5929666,53.0607896, SpatialReferences.Wgs84),new MapPoint(25.4597574,53.0071115, SpatialReferences.Wgs84),new MapPoint(25.3844511,52.9420694, SpatialReferences.Wgs84),new MapPoint(25.3169351,52.9376438, SpatialReferences.Wgs84),new MapPoint(25.3295195,52.8426463, SpatialReferences.Wgs84),new MapPoint(25.235911,52.8771825, SpatialReferences.Wgs84),new MapPoint(25.2084451,52.8440171, SpatialReferences.Wgs84),new MapPoint(25.1507669,52.8589447, SpatialReferences.Wgs84),new MapPoint(25.09606,52.8393283, SpatialReferences.Wgs84),new MapPoint(24.9482287,52.9414168, SpatialReferences.Wgs84),new MapPoint(24.6954062,52.9736231, SpatialReferences.Wgs84),new MapPoint(24.5995373,52.8847294, SpatialReferences.Wgs84),new MapPoint(24.5808512,52.7626549, SpatialReferences.Wgs84),new MapPoint(24.318401,52.764645, SpatialReferences.Wgs84),new MapPoint(24.13258,52.8160564, SpatialReferences.Wgs84),new MapPoint(23.9478865,52.7756079, SpatialReferences.Wgs84)
            };
            var mahouRivieraPolygon3 = new Polygon(polygonPoints3);

            // Create a fill symbol to display the polygon.
           var polygonSymbolOutline3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
         var   polygonFillSymbol3 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline3);


            var polygonGraphic3 = new Graphic(mahouRivieraPolygon3, polygonFillSymbol3);
            polygonGraphic3.IsVisible = false;
            regions.Add(polygonGraphic3);



             var polygonPoints4 = new List<MapPoint>
            {
                new MapPoint(27.5321884,52.2428884, SpatialReferences.Wgs84),new MapPoint(27.2685166,52.3470359, SpatialReferences.Wgs84),new MapPoint(27.3728867,52.4609808, SpatialReferences.Wgs84),new MapPoint(27.4442978,52.5512569, SpatialReferences.Wgs84),new MapPoint(27.6310654,52.5278698, SpatialReferences.Wgs84),new MapPoint(27.685997,52.4408943, SpatialReferences.Wgs84),new MapPoint(27.9881211,52.4375456, SpatialReferences.Wgs84),new MapPoint(28.0155869,52.5312116, SpatialReferences.Wgs84),new MapPoint(28.2902451,52.4910934, SpatialReferences.Wgs84),new MapPoint(28.4715195,52.6613427, SpatialReferences.Wgs84),new MapPoint(28.7846298,52.7279267, SpatialReferences.Wgs84),new MapPoint(29.0255085,52.8596677, SpatialReferences.Wgs84),new MapPoint(29.2287556,52.9689777, SpatialReferences.Wgs84),new MapPoint(29.3990437,52.9590518, SpatialReferences.Wgs84),new MapPoint(29.6407429,53.0416977, SpatialReferences.Wgs84),new MapPoint(29.6956745,53.1472539, SpatialReferences.Wgs84),new MapPoint(29.7835651,53.3379148, SpatialReferences.Wgs84),new MapPoint(30.1351276,53.3083852, SpatialReferences.Wgs84),new MapPoint(30.146114,53.213095, SpatialReferences.Wgs84),new MapPoint(30.4647175,53.2985374, SpatialReferences.Wgs84),new MapPoint(30.6295124,53.3182306, SpatialReferences.Wgs84),new MapPoint(31.0030476,53.2985374, SpatialReferences.Wgs84),new MapPoint(31.1348835,53.1604302, SpatialReferences.Wgs84),new MapPoint(31.3546101,53.1011049, SpatialReferences.Wgs84),new MapPoint(31.2886921,53.0416977, SpatialReferences.Wgs84),new MapPoint(31.5248981,52.8430815, SpatialReferences.Wgs84),new MapPoint(31.5908161,52.6202147, SpatialReferences.Wgs84),new MapPoint(31.5908161,52.4364121, SpatialReferences.Wgs84),new MapPoint(31.6732136,52.261927, SpatialReferences.Wgs84),new MapPoint(31.8105427,52.1204937, SpatialReferences.Wgs84),new MapPoint(31.6072956,52.1238664, SpatialReferences.Wgs84),new MapPoint(31.387569,52.1238664, SpatialReferences.Wgs84),new MapPoint(31.2117878,52.0428513, SpatialReferences.Wgs84),new MapPoint(31.0909382,52.0935029, SpatialReferences.Wgs84),new MapPoint(30.9041706,52.0327141, SpatialReferences.Wgs84),new MapPoint(30.639008,51.79071, SpatialReferences.Wgs84),new MapPoint(30.5511174,51.596624, SpatialReferences.Wgs84),new MapPoint(30.606049,51.398278, SpatialReferences.Wgs84),new MapPoint(30.5182868,51.2746441, SpatialReferences.Wgs84),new MapPoint(30.2656013,51.346753, SpatialReferences.Wgs84),new MapPoint(30.155738,51.4974711, SpatialReferences.Wgs84),new MapPoint(29.8975593,51.4666829, SpatialReferences.Wgs84),new MapPoint(29.7162849,51.4974711, SpatialReferences.Wgs84),new MapPoint(29.5075446,51.4769479, SpatialReferences.Wgs84),new MapPoint(29.485572,51.39819, SpatialReferences.Wgs84),new MapPoint(29.3317634,51.3707642, SpatialReferences.Wgs84),new MapPoint(29.1285163,51.6306469, SpatialReferences.Wgs84),new MapPoint(28.9802009,51.5521545, SpatialReferences.Wgs84),new MapPoint(28.8794091,51.5847316, SpatialReferences.Wgs84),new MapPoint(28.7860253,51.4787987, SpatialReferences.Wgs84),new MapPoint(28.7393334,51.4103241, SpatialReferences.Wgs84),new MapPoint(28.6596826,51.5642477, SpatialReferences.Wgs84),new MapPoint(28.4427026,51.5847316, SpatialReferences.Wgs84),new MapPoint(28.3493188,51.5471707, SpatialReferences.Wgs84),new MapPoint(28.2531884,51.6529446, SpatialReferences.Wgs84),new MapPoint(28.0856469,51.5693695, SpatialReferences.Wgs84),new MapPoint(27.9455712,51.5847316, SpatialReferences.Wgs84),new MapPoint(27.8466943,51.6239666, SpatialReferences.Wgs84),new MapPoint(27.7588036,51.4822197, SpatialReferences.Wgs84),new MapPoint(27.6544335,51.516416, SpatialReferences.Wgs84),new MapPoint(27.7230981,51.5847316, SpatialReferences.Wgs84),new MapPoint(27.5720361,51.6359009, SpatialReferences.Wgs84),new MapPoint(27.5096404,51.6183642, SpatialReferences.Wgs84),new MapPoint(27.5425994,51.7664867, SpatialReferences.Wgs84),new MapPoint(27.5661031,52.0047183, SpatialReferences.Wgs84),new MapPoint(27.5221578,52.1802149, SpatialReferences.Wgs84)
            };
            var mahouRivieraPolygon4 = new Polygon(polygonPoints4);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline4 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
            var polygonFillSymbol4 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline4);


            var polygonGraphic4 = new Graphic(mahouRivieraPolygon4, polygonFillSymbol4);
            polygonGraphic4.IsVisible = false;
            regions.Add(polygonGraphic4);



             var polygonPoints5 = new List<MapPoint>
            {
new MapPoint(28.3571188,52.7325173, SpatialReferences.Wgs84),new MapPoint(28.5367783,52.6674904, SpatialReferences.Wgs84),new MapPoint(28.9487656,52.8005383, SpatialReferences.Wgs84),new MapPoint(29.2069443,52.9398018, SpatialReferences.Wgs84),new MapPoint(29.4376572,52.9728939, SpatialReferences.Wgs84),new MapPoint(29.6738632,53.0819182, SpatialReferences.Wgs84),new MapPoint(29.7835651,53.3379148, SpatialReferences.Wgs84),new MapPoint(30.0968369,53.2925745, SpatialReferences.Wgs84),new MapPoint(30.1957138,53.2301439, SpatialReferences.Wgs84),new MapPoint(30.5143173,53.3122705, SpatialReferences.Wgs84),new MapPoint(30.9647568,53.3122705, SpatialReferences.Wgs84),new MapPoint(31.3527282,53.0753342, SpatialReferences.Wgs84),new MapPoint(31.479071,53.1775158, SpatialReferences.Wgs84),new MapPoint(31.6273864,53.2170047, SpatialReferences.Wgs84),new MapPoint(31.8416198,53.1017269, SpatialReferences.Wgs84),new MapPoint(32.149237,53.0885326, SpatialReferences.Wgs84),new MapPoint(32.3414978,53.1676379, SpatialReferences.Wgs84),new MapPoint(32.517279,53.2728851, SpatialReferences.Wgs84),new MapPoint(32.7260192,53.3090042, SpatialReferences.Wgs84),new MapPoint(32.7809509,53.4695273, SpatialReferences.Wgs84),new MapPoint(32.4403747,53.5870772, SpatialReferences.Wgs84),new MapPoint(32.4678405,53.714055, SpatialReferences.Wgs84),new MapPoint(32.2815617,53.7705256, SpatialReferences.Wgs84),new MapPoint(31.8036564,53.8094664, SpatialReferences.Wgs84),new MapPoint(31.8476017,53.8742875, SpatialReferences.Wgs84),new MapPoint(31.9190129,54.0455783, SpatialReferences.Wgs84),new MapPoint(31.8311222,54.0842634, SpatialReferences.Wgs84),new MapPoint(31.4594052,54.1938285, SpatialReferences.Wgs84),new MapPoint(31.3248226,54.2901285, SpatialReferences.Wgs84),new MapPoint(31.2424252,54.4389497, SpatialReferences.Wgs84),new MapPoint(31.1435482,54.464499, SpatialReferences.Wgs84),new MapPoint(30.684869,54.392601, SpatialReferences.Wgs84),new MapPoint(30.4775129,54.2897942, SpatialReferences.Wgs84),new MapPoint(30.2165876,54.3458659, SpatialReferences.Wgs84),new MapPoint(29.8512922,54.3154365, SpatialReferences.Wgs84),new MapPoint(29.6315656,54.3330562, SpatialReferences.Wgs84),new MapPoint(29.4388837,54.2446193, SpatialReferences.Wgs84),new MapPoint(29.4443769,54.1160275, SpatialReferences.Wgs84),new MapPoint(29.331767,54.0112521, SpatialReferences.Wgs84),new MapPoint(29.397685,53.8706001, SpatialReferences.Wgs84),new MapPoint(29.4745893,53.797663, SpatialReferences.Wgs84),new MapPoint(29.3729657,53.7327233, SpatialReferences.Wgs84),new MapPoint(29.2813533,53.6906151, SpatialReferences.Wgs84),new MapPoint(29.1247982,53.6727211, SpatialReferences.Wgs84),new MapPoint(29.020428,53.5880287, SpatialReferences.Wgs84),new MapPoint(28.9242977,53.5358257, SpatialReferences.Wgs84),new MapPoint(28.7924617,53.5961796, SpatialReferences.Wgs84),new MapPoint(28.6551326,53.5929194, SpatialReferences.Wgs84),new MapPoint(28.5178035,53.5472506, SpatialReferences.Wgs84),new MapPoint(28.3859676,53.4672116, SpatialReferences.Wgs84),new MapPoint(28.3763448,53.3468304, SpatialReferences.Wgs84),new MapPoint(28.381838,53.2779104, SpatialReferences.Wgs84),new MapPoint(28.0714742,53.2779104, SpatialReferences.Wgs84),new MapPoint(28.1346456,53.2039441, SpatialReferences.Wgs84),new MapPoint(28.3406393,53.1957178, SpatialReferences.Wgs84),new MapPoint(28.5109274,53.1430316, SpatialReferences.Wgs84),new MapPoint(28.5329,52.9845848, SpatialReferences.Wgs84),new MapPoint(28.4505026,52.8786295, SpatialReferences.Wgs84),new MapPoint(28.3571188,52.7325173, SpatialReferences.Wgs84)
            };
            var mahouRivieraPolygon5 = new Polygon(polygonPoints5);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline5 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
            var polygonFillSymbol5 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline5);


            var polygonGraphic5 = new Graphic(mahouRivieraPolygon5, polygonFillSymbol5);
            polygonGraphic5.IsVisible = false;
            regions.Add(polygonGraphic5);



             var polygonPoints6 = new List<MapPoint>
            {
new MapPoint(26.6581162,52.8801721, SpatialReferences.Wgs84),new MapPoint(26.8257734,52.7678435, SpatialReferences.Wgs84),new MapPoint(27.0152875,52.7695053, SpatialReferences.Wgs84),new MapPoint(27.0560512,52.6021735, SpatialReferences.Wgs84),new MapPoint(27.1082362,52.4685201, SpatialReferences.Wgs84),new MapPoint(27.2763624,52.3931051, SpatialReferences.Wgs84),new MapPoint(27.4158534,52.5337269, SpatialReferences.Wgs84),new MapPoint(27.517477,52.5487609, SpatialReferences.Wgs84),new MapPoint(27.6575527,52.4601533, SpatialReferences.Wgs84),new MapPoint(27.9486903,52.4534586, SpatialReferences.Wgs84),new MapPoint(28.0764446,52.5071945, SpatialReferences.Wgs84),new MapPoint(28.3071575,52.5038509, SpatialReferences.Wgs84),new MapPoint(28.4170208,52.6040485, SpatialReferences.Wgs84),new MapPoint(28.5186443,52.6573937, SpatialReferences.Wgs84),new MapPoint(28.3840618,52.7156656, SpatialReferences.Wgs84),new MapPoint(28.3812323,52.7798045, SpatialReferences.Wgs84),new MapPoint(28.5350409,52.9555517, SpatialReferences.Wgs84),new MapPoint(28.5109274,53.1430316, SpatialReferences.Wgs84),new MapPoint(28.3812323,53.1569464, SpatialReferences.Wgs84),new MapPoint(28.1346456,53.2039441, SpatialReferences.Wgs84),new MapPoint(28.0714742,53.2779104, SpatialReferences.Wgs84),new MapPoint(28.3263007,53.2753609, SpatialReferences.Wgs84),new MapPoint(28.4251777,53.4458267, SpatialReferences.Wgs84),new MapPoint(28.6551326,53.5929194, SpatialReferences.Wgs84),new MapPoint(28.9242977,53.5358257, SpatialReferences.Wgs84),new MapPoint(29.0327357,53.6302333, SpatialReferences.Wgs84),new MapPoint(29.2249964,53.6953313, SpatialReferences.Wgs84),new MapPoint(29.4745893,53.797663, SpatialReferences.Wgs84),new MapPoint(29.331767,54.0112521, SpatialReferences.Wgs84),new MapPoint(29.4388837,54.2446193, SpatialReferences.Wgs84),new MapPoint(29.345846,54.3792046, SpatialReferences.Wgs84),new MapPoint(29.5106409,54.6025465, SpatialReferences.Wgs84),new MapPoint(29.3073939,54.5547902, SpatialReferences.Wgs84),new MapPoint(29.1015469,54.6557849, SpatialReferences.Wgs84),new MapPoint(28.9916836,54.57308, SpatialReferences.Wgs84),new MapPoint(28.7031458,54.6025465, SpatialReferences.Wgs84),new MapPoint(28.5769497,54.6748467, SpatialReferences.Wgs84),new MapPoint(28.403915,54.5571559, SpatialReferences.Wgs84),new MapPoint(28.1977747,54.6566027, SpatialReferences.Wgs84),new MapPoint(28.0785573,54.6273817, SpatialReferences.Wgs84),new MapPoint(27.8626917,54.710587, SpatialReferences.Wgs84),new MapPoint(27.2859095,54.9352758, SpatialReferences.Wgs84),new MapPoint(26.9563196,54.9510518, SpatialReferences.Wgs84),new MapPoint(26.4894007,54.9573605, SpatialReferences.Wgs84),new MapPoint(26.2005218,55.0035572, SpatialReferences.Wgs84),new MapPoint(26.2630421,54.9470791, SpatialReferences.Wgs84),new MapPoint(26.533346,54.742309, SpatialReferences.Wgs84),new MapPoint(26.6432093,54.5738995, SpatialReferences.Wgs84),new MapPoint(26.5658301,54.3678717, SpatialReferences.Wgs84),new MapPoint(26.3900489,54.2332432, SpatialReferences.Wgs84),new MapPoint(26.1795386,54.2251572, SpatialReferences.Wgs84),new MapPoint(26.0181769,54.1592728, SpatialReferences.Wgs84),new MapPoint(26.1336341,54.0951219, SpatialReferences.Wgs84),new MapPoint(26.2417335,53.9820494, SpatialReferences.Wgs84),new MapPoint(26.3515967,54.0304741, SpatialReferences.Wgs84),new MapPoint(26.4247718,53.9547534, SpatialReferences.Wgs84),new MapPoint(26.2311377,53.8641505, SpatialReferences.Wgs84),new MapPoint(26.2959757,53.7165121, SpatialReferences.Wgs84),new MapPoint(26.5647324,53.5446507, SpatialReferences.Wgs84),new MapPoint(26.5482529,53.4237097, SpatialReferences.Wgs84),new MapPoint(26.3701334,53.3574078, SpatialReferences.Wgs84),new MapPoint(26.3703359,53.2639253, SpatialReferences.Wgs84),new MapPoint(26.3889511,53.1247915, SpatialReferences.Wgs84),new MapPoint(26.4180658,53.0032612, SpatialReferences.Wgs84),new MapPoint(26.446745,52.8143493, SpatialReferences.Wgs84),new MapPoint(26.6581162,52.8801721, SpatialReferences.Wgs84)
            };
            var mahouRivieraPolygon6 = new Polygon(polygonPoints6);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline6 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
            var polygonFillSymbol6 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline6);


            var polygonGraphic6 = new Graphic(mahouRivieraPolygon6, polygonFillSymbol6);
            polygonGraphic6.IsVisible = false;
            regions.Add(polygonGraphic6);
        }

        private async Task CreateSpiritMarkers()
        {
            var SpiritsGraphicOverlay = new GraphicsOverlay();
            polygonOverlay = new GraphicsOverlay();
            GraphicsOverlayCollection overlays = MainMapView.GraphicsOverlays;
            overlays.Add(polygonOverlay);
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
               // polygonOverlay.Graphics.Add(spirit.polygonGraphic);
                foreach(var region in regions)
                {
                    region.Geometry = region.Geometry.Project(spirit.mapPoint.SpatialReference);
                    if (spirit.mapPoint.Within(region.Geometry))
                    {
                        spirit.polygonGraphic = region;
                        break;
                    }
                }
                SpiritsGraphicOverlay.Graphics.Add(pinGraphic);
            }
            foreach (var polygon in regions)
            {
                polygonOverlay.Graphics.Add(polygon);
            }
        }
    }
}
