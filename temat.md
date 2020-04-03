Ok, w takim razie może takie coś:
```
class TestProvider
{
  public IEnumerable<TestEventArgs> Export(method, user_data);
  public IEnumerable<TestEventArgs> NWiseExport(method, n, coverage);
  public IEnumerable<TestEventArgs> CartesianExport(method, coverage);
  public IEnumerable<TestEventArgs> RandomExport(method, length, duplicates, adaptive);
  public IEnumerable<TestEventArgs> StaticExport(method, user_data);
}
```
Każda otrzymana wiadomość jest od razu przekazywana (TestEventArgs.DataRaw) a jeżeli można sparsować to jest to robione (TestEventArgs.TestCase, TestEventArgs.DataObject). Nie ma potrzeby rozróżniania na export i stream. Eventy bym zostawił, ponieważ są gotowe, powszechne w C#, i nic nas nie kosztują (jak użytkownik ich nie zarejestruje to i tak się nie wykonają). Będę ich bronił, bo z tego co wyczytałem to jak je usuniemy to pierwszym pytaniem na seminarium będzie "A czemu nie ma eventów?". No i do blokowania interfejsu w pętli przyda się wielowątkowość, eventy dobrze sobie z tym radzą. 

Jak wiadono, samego interfejsu nie można zwrócić, więc będziemy go zwracać jako klasę TestQueue, nie będzie ona jednak dostępne dla użytkownika (będzie widoczna tylko w formie IEnumerable). Mogę się zgodzić, że z TestList rezygnujemy, użytkownik sam sobie zrobi jak chce.

W tym systemie dzielimy projekt na dwa niezależne repozytoria - EcFeed i EcFeedNUnit. Pierwsze a nich nie jest od niczego zależne, a drugie zależy od NUnit i EcFeed. Nie da się mieć różnych zależności w jednym repozytorium.

```
class NunitTestProvider
{
  public IEnumerable<TestCaseData> Generate(method, user_data);
  public IEnumerable<TestCaseData> NWise(method, n, coverage);
  public IEnumerable<TestCaseData> Cartesian(method, coverage);
  public IEnumerable<TestCaseData> Random(method, length, duplicates, adaptive);
  public IEnumerable<TestCaseData> Static(method, user_data);
}
```

Rezygnujemy też z asynchroniczności w taskach, eventy i IEnumerable wystarczy.