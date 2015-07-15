# XMT-Scalper
Traduction du robot XMT Scalper

# Objectifs

Le Projet initial de [Capella](http://www.worldwide-invest.org) d'un robot de trading scalper XMT-Scalper v2.4.2 écrit en MQL4. Traduit en CSharp avec l'aide de la librairie [NQuotes](http://www.nquotes.net/) de [Daniel](support2@nquotes.net ).

A partir de là restructurer le code pour le rendre plus modulaire et ainsi plus lisible et ensuite le faire évoluer plus 
simplement.

# Installation

Pour compiler le programme avec Visual Studio, il faut au préalable :

1) installer [NQuotes](http://www.nquotes.net/installation)  
2) installer [metatrader 4](http://www.metatrader4.com/).
3) Modifier les instructions post-buids des deux projets MqlApiWithStdLib et XMT-Scalper
  
  remplacez dans 'xcopy "$(ProjectDir)$(OutDir)$(TargetFileName)" "C:\Users\{user}\AppData\Roaming\MetaQuotes\Terminal\{clé}\MQL4\Experts" /Y'

  votre propre clé unique d'installation de metatrader 4 qui peut être retrouvée dans le répertoire '%TERMINAL_DATA_PATH%' qui se trouve dans
  C:\Users\{user}\AppData\Roaming\MetaQuotes\Terminal\. et votre propre nom d'utilisateur windows.

  
