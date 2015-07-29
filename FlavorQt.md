# Introduction #

This is an educational project of UI course.

# Purpose #

Program can read spectrum files (xml format, up to 20 data point pair rows, information about the measure settings under which spectrum was retrieved); export them to pdf format; distract one spectrum from another (in the case of measure settings were equal); display these settings in side panel in ui.

# Class Descriptions #

## Widgets ##

`MainWindow` - main window class with mdi behaviour, has some controller features.
`SpectrumView` - displays spectrum.
`ParameterDockWidget` - displays measure settings of spectrum current displayed.

## Data Structures ##

`CommonOptions` - holds data of measure settings.
`PointPairListPlus` - holds data point pair row, contains an instance of `PreciseEditorData`.
`PreciseEditorData` - holds extra parameters of data point pair row.
`PreciseSpectrum` - contains list of `PointPairListPlus` and an instance of `CommonOptions`.

## Model ##

`CommonOptionsTableModel` - underlays `ParameterDockWidget`.

## Extra ##

`Util` - a namespace for service (de)serialization methods.

# Compiling Requirements #

Project compilation needs Qwt 5.2.1 (http://qwt.sourceforge.net/ ; svn co https://qwt.svn.sourceforge.net/svnroot/qwt/branches/qwt-5.2) installed. After that INCLUDEPATH and LIBS variables in flavor-ms.pro may need to be corrected correspondingly to Qwt installation path.

# Test samples #

A few spectrum files for testing purposes can be found in test\_spectrums directory under the project root.