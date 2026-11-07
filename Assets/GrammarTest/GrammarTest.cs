// General
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// DryWet MIDI
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Standards; // not used
// Aliases
using Chord = Melanchall.DryWetMidi.MusicTheory.Chord;
using Note = Melanchall.DryWetMidi.MusicTheory.Note;

public class GrammarTest: MonoBehaviour
{
    #region Unity Testing

    private AudioPlayer_Basic _player;
    private void Start()
    {
        //gets chord sequence
        List<List<Chord_CommonNotation>> chordsNotation = GetSectionChordSequence_Ionian();
        //converts to discrete chords
        List<Chord> chordsList = new();
        for(int i = 0; i < chordsNotation.Count; i++){
            for(int j = 0; j < chordsNotation[i].Count; j++){
                chordsList.Add(GetDiatonicChord_Triad(chordsNotation[i][j], GetDiatonicScale(NoteName.C, DiatonicMode.Ionian)));
            }
        }
        //builds chords
        PatternBuilder builder = new();
        for(int i = 0; i < chordsList.Count; i++){
           builder = WriteChordPattern_Block(builder, chordsList[i], MusicalTimeSpan.Whole, 3, 0);
        }
        //staple patterns together
        Pattern chordSequence = builder.Build();
        //set up and start playback
        _player = new() {
            Pattern = chordSequence,
            Loop = true
        };
        _player.Play();

    }

    private void OnApplicationQuit()
    {
        _player.Dispose();
    }

    #endregion

    /* Grammar Notes
    the macrostructure becomes symbols, A, B, C, D.
    These are not true gramatical symbols, but bins, each with their own orgin symbols.
    once an A, B is generated, subsequent bins will match
    therefore for ABAB, only one A, and one B needs to actually be generated (for now)
    we dont do development (in the musical sense)

    within the A, Bs we have a small sequence of orgin symbols, genererated for now arbitrarialy
    these are all 'c' or 'o', closed or open
    c and o becomes a sequence of function-categorical symbols.
    these symbols are T, Ts, D, Ds, SD
    c always ends on a tonic, or tonic sub. 
    o always ends on a dominant or dminant sub

    we then for each of these categories, substitute a chord arbitrarialy for the set.

    Technical considerations on symbols
     - for c, and o, booleans can be used or an enumeration (for extendability later)
     - for T,Ts,D,Ds,SD, a enumeration can be used
     - for the discrete chords, integers could be used, mapped to roman numerals, or enumerations
         - we will need to map these to chords later. Enums can have function extensions that may be useful
    */

    #region Enumerations

    /// <summary>
    /// Grammar component. Indicates if a passage is open or closed
    /// </summary>
    public enum PassageOpen { Open, Closed }
    /// <summary>
    /// Grammar component. Maps to functional harmony
    /// </summary>
    public enum FunctionCategory { Tonic = 0, TonicSub = 1, Dominant = 2, DominantSub = 3, Subdominant = 4 }
    /// <summary>
    /// Grammar component. Maps to discrete chords within a scale, by degree. 
    /// </summary>
    public enum Chord_CommonNotation { I = 0, II = 1, III = 2, IV = 3, V = 4, VI = 5, VII = 6 }
    /// <summary>
    /// Diatonic modes. Libraries do not come with anything for these. Ordered from "darkest" to "lightest". Locrian = 0, Lydian = 6. 
    /// </summary>
    public enum DiatonicMode { Locrian = 0, Phrygian = 1, Aeolian = 2, Dorian = 3, Mixolydian = 4, Ionian = 5, Lydian = 6, }

    #endregion

    #region Grammar Data

    // sequences of orgin symbols
    private readonly List<List<PassageOpen>> _orginSequences = new(){ 
        new() {PassageOpen.Closed, PassageOpen.Closed }, //cc
        new() {PassageOpen.Closed, PassageOpen.Open }, //co
        new() {PassageOpen.Closed, PassageOpen.Open, PassageOpen.Closed }, //coc
        new() {PassageOpen.Closed, PassageOpen.Open, PassageOpen.Open, PassageOpen.Closed }, //cooc
    };
    // function mapping for Ionian with common chords
    private readonly Dictionary<FunctionCategory, List<Chord_CommonNotation>> _ionianFunctionalBindings = new(){
        { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
        { FunctionCategory.TonicSub, new() {Chord_CommonNotation.III, Chord_CommonNotation.VI, } },
        { FunctionCategory.Dominant, new() {Chord_CommonNotation.V } },
        { FunctionCategory.DominantSub, new() {Chord_CommonNotation.V, Chord_CommonNotation.VII, } },
        { FunctionCategory.Subdominant, new() {Chord_CommonNotation.II, Chord_CommonNotation.IV } },
    };

    #endregion

    #region Grammars

    /// <summary>
    /// Generates the sequence of chord functions procedurally from the passage status
    /// </summary>
    /// <param name="passageOpen"> is the passage open or closed? </param>
    /// <returns> randomly generated set of functional categories in sequence that satisfy passage condition </returns>
    public List<FunctionCategory> GenerateFunctionSequence(PassageOpen passageOpen)
    {
        // algorithm is not specific for quantity. We should seek to generate four chords, though 5 is acceptable.
        // the last chord is the only constrained chord.
        int CHORDQUANTITY = 4;
        List<FunctionCategory> chordFunctions = new();
        System.Random rand = new();
        for (int i = 0; i < CHORDQUANTITY - 1; i++) { chordFunctions.Add((FunctionCategory)rand.Next(0, 5)); }
        if (passageOpen == PassageOpen.Open){
            chordFunctions.Add(rand.Next(0, 2) == 0 ? FunctionCategory.Dominant : FunctionCategory.DominantSub);
        }
        if (passageOpen == PassageOpen.Closed){
            chordFunctions.Add(rand.Next(0, 2) == 0 ? FunctionCategory.Tonic : FunctionCategory.TonicSub);
        }

        return chordFunctions;
    }
    /// <summary>
    /// Runs grammars through fully for Ionian 
    /// </summary>
    /// <returns> A list of lists of chords. first-order lists specify passages. second-order is that passages chord sequence </returns>
    public List<List<Chord_CommonNotation>> GetSectionChordSequence_Ionian()
    {
        System.Random rand = new();
        // gets orgin symbols
        List<PassageOpen> passageOpenSequence = _orginSequences[rand.Next(0, _orginSequences.Count)];
        // gets functional sequences
        List<List<FunctionCategory>> categorySets = new();
        for (int i = 0; i < passageOpenSequence.Count; i++) { categorySets.Add(GenerateFunctionSequence(passageOpenSequence[i])); }
        List<List<Chord_CommonNotation>> output = new();
        //converts functions to common chords
        for (int i = 0; i < categorySets.Count; i++){
            output.Add(new());
            for (int j = 0; j < categorySets[i].Count; j++){
                output[^1].Add(_ionianFunctionalBindings[categorySets[i][j]][rand.Next(0, _ionianFunctionalBindings[categorySets[i][j]].Count)]);
            }
        }

        return output;
    }

    #endregion

    #region Scales and Chords

    /// <summary>
    /// Get a diatonic scale via root note, and mode specification
    /// </summary>
    /// <param name="root"> root note for the diatonic scale </param>
    /// <param name="scaleMode"> mode for the scale </param>
    /// <returns></returns>
    public Scale GetDiatonicScale(NoteName root, DiatonicMode scaleMode = DiatonicMode.Ionian)
    {
        /* Theory behind algorithm
        Modes rotate intervals     | Content | Index-Reading
        Lydian     : W-W-W-H-W-W-H | -       | -
        Ionian     : W-W-H-W-W-W-H | + 3     | + 4
        Mixolydian : W-W-H-W-W-H-W | + 3     | + 4
        etc. continutes on for other darker modes
        Intervals start 3 later, in mod 7
        */

        // Implementation
        Interval[] lydianIntervals = new Interval[7] { Interval.Two, Interval.Two, Interval.Two, Interval.One, Interval.Two, Interval.Two, Interval.One };
        Interval[] scaleIntervals = new Interval[7];
        int index = (6 - (int)scaleMode) * 4 % 7; //gets starting index for interval loop
        for(int i = 0; i < scaleIntervals.Length; i++){ //populates interval sequence
            scaleIntervals[i] = lydianIntervals[index];
            index = (index + 1) % 7;
        }
        return new Scale(scaleIntervals, root);
    }

    /// <summary>
    /// Get a triadic chord from a diatonic scale
    /// </summary>
    /// <param name="chord"> chord to get </param>
    /// <param name="scale"> Scale to read from </param>
    /// <returns> chord of specified root </returns>
    public Chord GetDiatonicChord_Triad(Chord_CommonNotation chord, Scale scale)
    {
        Note[] scaleNotes = scale.GetAscendingNotes(Note.Get(scale.RootNote, 4)).ToArray();
        int startIndex = (int)chord;
        return new(
            scaleNotes[startIndex].NoteName,
            scaleNotes[(startIndex + 2) % scaleNotes.Length].NoteName,
            scaleNotes[(startIndex + 4) % scaleNotes.Length].NoteName
        );
    }

    #endregion

    #region Audio Output Testing

    /// <summary>
    /// Writes a chord as a close block to a pattern builder, shifts playhead to end of what it writes
    /// </summary>
    /// <param name="builder"> builder to add to </param>
    /// <param name="chord"> chord to write </param>
    /// <param name="chordtimeSpan"> length of block chord </param>
    /// <param name="bassOctave"> octave of the lowest note </param>
    /// <param name="inversion"> inversion for the chord </param>
    /// <returns> pattern builder with the block chord appended </returns>
    public PatternBuilder WriteChordPattern_Block(PatternBuilder builder, Chord chord, MusicalTimeSpan chordtimeSpan, int bassOctave = 3, int inversion = 0)
    {
        NoteName[] noteNames = chord.NotesNames.ToArray();
        int selectionIndex = Mathf.Clamp(inversion, 0, noteNames.Length - 1);
        int octave = bassOctave;
        for (int i = 0; i < noteNames.Length; i++){
            // writes the note of the chord
            builder.Note(Note.Get(noteNames[selectionIndex], octave), chordtimeSpan, (SevenBitNumber)100);
            builder.StepBack(chordtimeSpan);
            // octave change checking
            if ((int)noteNames[i] > (int)noteNames[(selectionIndex + 1) % noteNames.Length]) { octave++; }
            //advance note selection in chord
            selectionIndex = (selectionIndex + 1) % noteNames.Length;
        }
        builder.StepForward(chordtimeSpan); // move to end of what was written

        return builder;
    }

    /// <summary>
    /// A basic audio player for testing, requires Microsoft GS Wavetable Synth
    /// </summary>
    public class AudioPlayer_Basic: IDisposable
    {
        public Pattern Pattern;
        public bool Loop;

        private Playback _playback;
        private MidiFile _outputFile;
        private OutputDevice _outputDevice;
        private const string OUTPUT_DEVICE_NAME = "Microsoft GS Wavetable Synth";

        public void Play()
        {
            if (Pattern == null) return;

            _outputFile = Pattern.ToFile(TempoMap.Create(Tempo.Default));
            _outputDevice = OutputDevice.GetByName(OUTPUT_DEVICE_NAME);
            _playback = _outputFile.GetPlayback(_outputDevice);
            _playback.Loop = Loop;
            _playback.MoveToTime(new MetricTimeSpan(0));
            _playback.Start();
        }
        public void Stop()
        {
            _playback?.Stop();
        }

        public void Dispose()
        {
            _playback?.Dispose();
            _outputDevice?.Dispose();
        }

    }

    #endregion

}
