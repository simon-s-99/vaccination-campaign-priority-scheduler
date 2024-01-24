# Vaccination Campaign Priority & Scheduling - Assignment

## Överblick
Skriv ett interaktivt konsolprogram som skapar en prioritetsordning för en hypotetisk vaccinationskampanj, enligt beskrivningen nedan.

*Observera: Denna uppgift är baserad på vaccinationskampanjen för covid men många detaljer skiljer sig, så utgå enbart från beskrivningen nedan.*

## Funktionalitet
Programmet ska ha en huvudmeny som låter användaren dels ändra ett antal inställningar och dels skapa en prioritetsordning utifrån de inställningar som har gjorts. Menyval som låter användaren schemalägga vaccinationerna för personerna i prioritetsordningen. När prioritetsordningen skapas ska indatan läsas från en CSV-fil på datorn och utdatan sparas i en annan CSV-fil på datorn. 

## Regler för prioritetsordning
Befolkningen ska vaccineras i fyra faser enligt följande ordning:

1. Samtliga personer som är anställda inom vård och omsorg.
2. Samtliga kvarvarande personer som är 65 år eller äldre.
3. Samtliga kvarvarande personer som tillhör en riskgrupp.
4. Samtliga kvarvarande personer.

Följande regler ska också gälla:

- Inom varje fas ska vaccineringarna tilldelas efter ålder: äldst först. Även månad och dag ska tas i med denna beräkning (inte bara år).
- Personer yngre än 18 år ska enbart vaccineras om användaren väljer detta som alternativ (se exemplet nedan). Om detta alternativ väljs ska dessa personer ingå i de fyra faserna enligt samma regler som för alla andra. Om detta alternativ **inte** väljs ska ingen person yngre än 18 vaccineras oavsett omständigheterna, även om de exempelvis tillhör en riskgrupp.
- Personer som redan har genomgått en infektion ska vaccineras med enbart en dos. Övriga ska vaccineras med två doser.
- Om det enbart finns en dos kvar och nästa person i ordningen ska vaccineras med två doser så ska denna person **inte** tilldelas några doser. Kvarvarande personer ska inte heller tilldelas några doser, även om någon av dem bara ska vaccineras med en dos (på grund av genomgången infektion). Med andra ord ska den sista dosen antingen användas till en fullständig vaccination av nästa person i ordningen eller inte användas alls.
- Antalet tillgängliga vaccindoser ska inte ändras efter att en prioritetsordning har skapats. Denna inställning ska alltså förbli samma oavsett hur många prioritetsordningar som skapas, tills användaren själv ändrar antalet tillgängliga vaccindoser med det aktuella menyvalet.

## CSV-format (indata)
CSV-filerna med programmets indata ska beskriva personer i en befolkning och innehålla följande kolumner:

1. Personnummer; två eller fyra siffror för årtal och valfritt bindestreck före de fyra sista siffrorna
    - För personnummer med två siffror för årtal kan programmet anta att århundradet är 1900.
2. Efternamn
3. Förnamn
4. Huruvida personen är anställd inom vård och omsorg: `0` för nej, `1` för ja
5. Huruvida personen tillhör en riskgrupp: `0` för nej, `1` för ja
6. Huruvida personen redan har genomgått en infektion: `0` för nej, `1` för ja

## CSV-format (utdata)
CSV-filerna som programmets utdata sparas i ska beskriva vaccinationer sorterade i prioritetsordning (högst prioritet längst upp) och innehålla följande kolumner:

1. Personnummer; fyra siffror för årtal och bindestreck före de fyra sista siffrorna
2. Efternamn
3. Förnamn
4. Antalet vaccindoser som personen ska få (1 eller 2)

## Regler för schemaläggning
- Den första vaccinationen ska äga rum på valfritt datum som användaren matar in.
- 2 personer åt gången kan vaccineras.
- Varje vaccination tar 5 minuter.
- Vaccinering ska pågå oavbrutet i samma takt från 8:00 till 20:00 varje dag, inklusive lördagar och söndagar.
- Schemat ska endast innefatta den första dosen för varje person.
- Schemat ska sparas i en `.ics`-fil (se detaljer nedan).
- Det är **inte** tillåtet att använda externa bibliotek för detta, utan koden för att hantera iCalendar måste ni skriva själva.

Följande värden ska kunna bestämmas av användaren:

- Vilket datum som den första vaccinationen ska äga rum på. (Standardvärde: en vecka efter nuvarande datum)
- Starttid för vaccinationer. (Standardvärde: 8:00)
- Sluttid för vaccinationer. (Standardvärde: 20:00)
- Hur många personer som kan vaccineras åt gången. (Standardvärde: 2)
- Hur lång tid en vaccination tar. (Standardvärde: 5 minuter)
- Var filen ska sparas. (Standardvärde: C:\Windows\Temp\Schedule.ics)
  
## Utdata
Utdatan från schemaläggningen ska sparas i en iCalendar-fil med filändelsen `.ics`.

## Felhantering
Vid inmatning av data ska fel hanteras så att programmet inte kraschar:

- Vid inläsning av heltal: be om ny inmatning om ett ogiltigt värde matas in.
- Vid inläsning av sökväg till indatafil: be om ny inmatning om den angivna filen inte finns.
- Vid inläsning av sökväg till utdatafil: be om ny inmatning om den angivna **mappen** inte finns.
    - Det är OK om **filen** inte finns; den ska i så fall skapas när programmet körs.
- Vid inläsning av CSV-fil: skriv ut ett felmeddelande för varje felaktig rad i CSV-filen och återgå till huvudmenyn istället för att skapa en prioritetsordning.
    - Ett exempel på en felaktig rad är en som saknar någon kolumn (exempelvis efternamn) eller som innehåller ett ogiltigt värde i någon kolumn (exempelvis något annat värde än `1` eller `0` i de sista tre kolumnerna).
    - För personnummer behöver ni enbart kontrollera att de matchar formatet som beskrivs ovan; ni behöver inte förhindra dem från att innehålla ogiltiga datum (exempelvis 30:e februari).
- Vid skapande av prioritetsordning: om utdatafilen redan existerar så ska användaren bekräfta om de vill skriva över den.
- Inläsning av datum
- Inläsning av klockslag

## Tester
Skriv åtminstone **5** tester (högst 10) som testar en metod i ert program med olika intressanta scenarion.

`public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)`

Denna metod tar emot en array av strängar som ska innehålla raderna i indatafilen och returnerar en array av strängar som ska innehålla raderna i utdatafilen. Denna metod måste användas i både testerna och i huvudkoden, men exakt hur och var ni anropar den i huvudkoden är upp till er.
