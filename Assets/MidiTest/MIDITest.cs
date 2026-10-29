using UnityEngine;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards; // not used
using Melanchall.DryWetMidi.MusicTheory;
using Note = Melanchall.DryWetMidi.MusicTheory.Note; //alias for notes used in pattern builder, readability and verbosity

/// <summary>
/// Test of WetDry Midi Library
/// </summary>
public class MIDITest : MonoBehaviour
{
    private MidiFile _outputFile; 
    private PatternBuilder _patternBuilder;
    private Playback _playback;
    private OutputDevice _outputDevice;
    private const string OUTPUT_DEVICE_NAME = "Microsoft GS Wavetable Synth";

    void Start()
    {
        _patternBuilder = new(); // mini-ABAB' :)
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).C, Octave.Get(3).E, Octave.Get(3).G);
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).C, Octave.Get(3).E, Octave.Get(3).G);
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).D, Octave.Get(3).F, Octave.Get(3).A);
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).D, Octave.Get(3).F, Octave.Get(3).A);
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).C, Octave.Get(3).E, Octave.Get(3).G);
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).C, Octave.Get(3).E, Octave.Get(3).G);
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).D, Octave.Get(3).F, Octave.Get(3).A);
        _patternBuilder = AddWaltzChordBar(_patternBuilder, Octave.Get(3).D, Octave.Get(3).F, Octave.Get(3).A, Octave.Get(3).F, Octave.Get(3).GSharp);
        //padding, inaudible. for clean looping
        _patternBuilder.StepBack(MusicalTimeSpan.Sixteenth);
        _patternBuilder.Note(Octave.Get(0).A, MusicalTimeSpan.Sixteenth, (SevenBitNumber)0);
        // Melody :)
        _patternBuilder = AppendWaltzMelody(_patternBuilder);

        // sets to file and plays
        _outputFile = _patternBuilder.Build().ToFile(TempoMap.Create(Tempo.FromBeatsPerMinute(156)));
        _outputDevice = OutputDevice.GetByName(OUTPUT_DEVICE_NAME);
        _playback = _outputFile.GetPlayback(_outputDevice);
        _playback.Loop = true; // there is no escape
        _playback.Start(); // :)
    }

    // Macro for creating the standard Waltz left-hand pattern
    // a is the single note root. A and B are the first two notes on beat 2. C and D are these for beat 3.
    // A -> C, if C null. B -> D if D Null
    private PatternBuilder AddWaltzChordBar(PatternBuilder builder, Note root, Note A, Note B, Note C = null, Note D = null)
    {
        builder.Note(root, MusicalTimeSpan.Quarter, (SevenBitNumber)100);
        _patternBuilder.Note(A, MusicalTimeSpan.Eighth, (SevenBitNumber)65);
        _patternBuilder.StepBack(MusicalTimeSpan.Eighth);
        _patternBuilder.Note(B, MusicalTimeSpan.Eighth, (SevenBitNumber)65);
        _patternBuilder.StepForward(MusicalTimeSpan.Eighth);
        if(C != null) { A = C; }
        if(D != null) { B = D; }
        _patternBuilder.Note(A, MusicalTimeSpan.Eighth.Dotted(1), (SevenBitNumber)80);
        _patternBuilder.StepBack(MusicalTimeSpan.Eighth.Dotted(1));
        _patternBuilder.Note(B, MusicalTimeSpan.Eighth.Dotted(1), (SevenBitNumber)80);
        _patternBuilder.StepForward(MusicalTimeSpan.Sixteenth);

        return builder; //not neccesary but reduncancy can help track whats going on
    }

    // melody :)
    private PatternBuilder AppendWaltzMelody(PatternBuilder builder)
    {
        builder.MoveToStart();

        // A
        // Bar 1 
        _patternBuilder.Note(Octave.Get(4).C, MusicalTimeSpan.Half, (SevenBitNumber)95);
        _patternBuilder.Note(Octave.Get(4).B, MusicalTimeSpan.Eighth, (SevenBitNumber)60);
        _patternBuilder.Note(Octave.Get(4).C, MusicalTimeSpan.Eighth, (SevenBitNumber)70);
        // Bar 2
        _patternBuilder.Note(Octave.Get(4).D, MusicalTimeSpan.Half, (SevenBitNumber)105);
        _patternBuilder.Note(Octave.Get(4).F, MusicalTimeSpan.Eighth, (SevenBitNumber)65);
        _patternBuilder.Note(Octave.Get(4).E, MusicalTimeSpan.Eighth, (SevenBitNumber)80);
        // B
        // Bar 3
        _patternBuilder.Note(Octave.Get(4).D, MusicalTimeSpan.Quarter, (SevenBitNumber)90);
        _patternBuilder.Note(Octave.Get(4).E, MusicalTimeSpan.Quarter, (SevenBitNumber)90);
        _patternBuilder.Note(Octave.Get(4).F, MusicalTimeSpan.Quarter, (SevenBitNumber)90);
        // Bar 4
        _patternBuilder.Note(Octave.Get(4).A, MusicalTimeSpan.Quarter, (SevenBitNumber)90);
        _patternBuilder.Note(Octave.Get(4).F, MusicalTimeSpan.Quarter, (SevenBitNumber)90);
        _patternBuilder.Note(Octave.Get(4).D, MusicalTimeSpan.Quarter, (SevenBitNumber)90);
        // A
        // Bar 5
        _patternBuilder.Note(Octave.Get(4).C, MusicalTimeSpan.Half.Dotted(1), (SevenBitNumber)95);
        // Bar 6
        _patternBuilder.Note(Octave.Get(4).G, MusicalTimeSpan.Half.Dotted(1), (SevenBitNumber)95);
        // B'
        // Bar 7
        _patternBuilder.Note(Octave.Get(4).F, MusicalTimeSpan.Half.Dotted(1), (SevenBitNumber)95);
        // Bar 8
        _patternBuilder.Note(Octave.Get(4).A, MusicalTimeSpan.Quarter, (SevenBitNumber)90);
        _patternBuilder.Note(Octave.Get(4).F, MusicalTimeSpan.Quarter, (SevenBitNumber)70);
        _patternBuilder.Note(Octave.Get(4).D, MusicalTimeSpan.Quarter, (SevenBitNumber)80);

        return _patternBuilder;
    }

    // (From Demo) THIS IS IMPORTANT! will not stop playback otherwise when application is quit, and will cause unity to crash
    private void OnApplicationQuit()
    {
        if (_playback != null) { _playback.Dispose(); }
        if (_outputDevice != null) { _outputDevice.Dispose(); }       
    }
}
