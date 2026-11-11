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

    [SerializeField] private int _seedValue;
    [SerializeField] [Range(-1,1)] private float _valence;
    [SerializeField] [Range(0,1)] private float _arousal;
    [SerializeField] private NoteName _scaleRoot;
    [SerializeField] private char[] _sectionIdentities = new char[0];

    private AudioPlayer_Basic _player;
    private void Start()
    {
        //gets chord sequence
        Seed seed = new(); seed.SetSeed(_seedValue);
        Piece piece = new() { Valence = _valence, Arousal = _arousal, 
        ChordDuration = MusicalTimeSpan.Eighth, ScaleRoot = _scaleRoot,
        Seed = seed, SectionIdentitySequence = _sectionIdentities };
        Pattern piecePattern = piece.GeneratePiece();

        //set up and start playback
        _player = new() {
            Pattern = piecePattern,
            Loop = true
        };
        _player.Play();

    }

    private void OnApplicationQuit()
    {
        _player.Dispose();
    }

    #endregion


    
    #region Enumerations

    /// <summary>
    /// Grammar component. Indicates if a passage is open or closed
    /// </summary>
    public enum PassageClosure { Open, Closed }
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


    #region Audio Output Testing

    /// <summary>
    /// For Writing chords to a pattern in a pattern builder
    /// </summary>
    public class ChordWriter
    {
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
            for (int i = 0; i < noteNames.Length; i++)
            {
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


    #region High-Level Structures

    /// <summary>
    /// Data structure that reperesents a section of a piece (The 'A' in an ABAB form)
    /// </summary>
    public struct Section
    {
        /// <summary>
        /// Optional, seed for the section
        /// </summary>
        public Seed Seed;

        // Melody Integration (Once Max finishes it)



        // Harmony/Grammar functions

        /// <summary>
        /// Runs grammars through fully, obtaining the harmonic sequence for the section as a list of lists of chords in common notation. 
        /// Sublists specify "passage", from orgin symbols
        /// </summary>
        /// <param name="mode"> Diatonic mode of the section </param>
        /// <param name="seed"> seed if used. Incremented by function. </param>
        /// <returns> A list of lists of chords. first-order lists specify passages. second-order is that passages chord sequence </returns>
        public readonly List<List<Chord_CommonNotation>> GetChordSequence(DiatonicMode mode, Seed seed )
        {
            // seed the random number gen, and increment seed
            System.Random rand = Seed.GetSeededGenerator(seed);
            // gets orgin symbols and functional sequences
            List<List<FunctionCategory>> categorySets = new();
            List<PassageClosure> orginSymbols = FunctionalClosureBindings.GetRandomClosureSequence(seed);
            for (int i = 0; i < orginSymbols.Count; i++){
                categorySets.Add(GenerateFunctionSequence(orginSymbols[i], seed));
            }
            // converts functions to common chords
            List<List<Chord_CommonNotation>> output = new();
            Dictionary<FunctionCategory, List<Chord_CommonNotation>> modeFuncBindings = ModalChordFunctionBindings.GetModeFunctionCommonChordSet(mode);
            for (int i = 0; i < categorySets.Count; i++){
                output.Add(new());
                for (int j = 0; j < categorySets[i].Count; j++){
                    output[^1].Add(modeFuncBindings[categorySets[i][j]][rand.Next(0, modeFuncBindings[categorySets[i][j]].Count)]);
                }
            }

            return output;

            // Generates the sequence of chord functions procedurally from the passage status
            static List<FunctionCategory> GenerateFunctionSequence(PassageClosure passageOpen, Seed seed)
            {
                // algorithm is not specific for quantity. We should seek to generate four chords, though 5 is acceptable.
                // the last chord is the only constrained chord.
                int CHORDQUANTITY = 4;
                List<FunctionCategory> chordFunctions = new();
                System.Random rand = Seed.GetSeededGenerator(seed); // applies seed, if supplied
                for (int i = 0; i < CHORDQUANTITY - 1; i++) { chordFunctions.Add((FunctionCategory)rand.Next(0, 5)); }
                if (passageOpen == PassageClosure.Open){
                    chordFunctions.Add(rand.Next(0, 2) == 0 ? FunctionCategory.Dominant : FunctionCategory.DominantSub);
                }
                if (passageOpen == PassageClosure.Closed){
                    chordFunctions.Add(rand.Next(0, 2) == 0 ? FunctionCategory.Tonic : FunctionCategory.TonicSub);
                }

                return chordFunctions;
            }
        }

    }

    /// <summary>
    /// Data structure that represents a collection of sections, represents a full generated piece
    /// </summary>
    public struct Piece
    {
        /// <summary>
        /// Optional seed for the section
        /// </summary>
        public Seed Seed;
        /// <summary>
        /// Valence value, [-1, 1]
        /// </summary>
        public float Valence;
        /// <summary>
        /// Arousal value, [0, 1]
        /// </summary>
        public float Arousal;

        /// <summary>
        /// High-Level section stricture, ie 'ABAB', 'ABAC', 'ABACAD', etc.
        /// </summary>
        public char[] SectionIdentitySequence;

        /// <summary>
        /// Root note of the scale used. For modes, tonic of the mode. If C, C Ionian, C Dorian, etc. 
        /// </summary>
        public NoteName ScaleRoot;
        /// <summary>
        /// Musical length of each chord (half note, whole note, etc.)
        /// </summary>
        public MusicalTimeSpan ChordDuration;

        public Pattern PieceGenerated{
            get { 
                _pieceGenerated??= GeneratePiece();
                return _pieceGenerated;
            }
            private set { _pieceGenerated = value; }
        }
        private Pattern _pieceGenerated;


        private List<Section> _sections;

        private void GeneratePieceSections()
        {
            _sections = new();
            Dictionary<int, Section> identityMap = new();
            for (int i = 0; i < SectionIdentitySequence.Length; i++){
                if (identityMap.ContainsKey(SectionIdentitySequence[i])){
                    _sections.Add(identityMap[SectionIdentitySequence[i]]);
                }
                else{
                    identityMap.Add(SectionIdentitySequence[i], new() { Seed = Seed });
                    _sections.Add(identityMap[SectionIdentitySequence[i]]);
                }
            }
        }

        public Pattern GeneratePiece()
        {
            GeneratePieceSections();
            PatternBuilder patternBuilder = new();
            DiatonicMode mode = MusicGenUtility.GetModeFromValence(Valence);
            Scale scale = MusicGenUtility.GetDiatonicScale(ScaleRoot, mode);
            ChordWriter chordWriter = new();
            for(int i = 0; i < _sections.Count; i++){
                List<List<Chord_CommonNotation>> chords = _sections[i].GetChordSequence(mode, Seed);
                for(int j = 0; j < chords.Count; j++){
                    for (int k = 0; k < chords[j].Count; k++){
                        Chord chord = MusicGenUtility.GetDiatonicChord_Triad(chords[j][k], scale);
                        chordWriter.WriteChordPattern_Block(patternBuilder, chord, ChordDuration, 3, 0);
                    }
                }
            }

            return patternBuilder.Build();
        }

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

    #region Grammar Data

    /// <summary>
    /// Contains hardcoded bindings for functional harmony within each diatonic mode 
    /// </summary>
    public static class ModalChordFunctionBindings
    {
        #region Public Retreival Functions

        /// <summary>
        /// Get the common triadic chords assigned to each function in a diatonic mode. 
        /// Cloned dictionary, caller should cache result if needed later.
        /// </summary>
        /// <param name="mode"> mode to retreive chord functions for </param>
        /// <returns> clone of the binding dictionary for common chords </returns>
        public static Dictionary<FunctionCategory, List<Chord_CommonNotation>> GetModeFunctionCommonChordSet(DiatonicMode mode)
        {
            switch (mode)
            {
                case DiatonicMode.Locrian: return _locrianFunctionalBindings.Clone().CommonChordBindings;
                case DiatonicMode.Phrygian: return _phrygianFunctionalBindings.Clone().CommonChordBindings;
                case DiatonicMode.Aeolian: return _aeolianFunctionalBindings.Clone().CommonChordBindings;
                case DiatonicMode.Dorian: return _dorianFunctionalBindings.Clone().CommonChordBindings;
                case DiatonicMode.Mixolydian: return _mixolydianFunctionalBindings.Clone().CommonChordBindings;
                case DiatonicMode.Ionian: return _ionianFunctionalBindings.Clone().CommonChordBindings;
                case DiatonicMode.Lydian: return _lydianFunctionalBindings.Clone().CommonChordBindings;
                default:
                    Debug.LogError("Invalid Entry to GetModeFunctionCommonChordSet. Returning Ionian's bindings");
                    return _ionianFunctionalBindings.Clone().CommonChordBindings;
            }
        }


        #endregion

        #region Static Defenitions for Modal Function Bindings

        // function mapping for Lydian with common chords
        private static readonly ScaleChordFunctionBinding _lydianFunctionalBindings = new(){
            CommonChordBindings = new() {
                { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
                { FunctionCategory.TonicSub, new() {Chord_CommonNotation.I, Chord_CommonNotation.VI, } },
                { FunctionCategory.Dominant, new() {Chord_CommonNotation.II } },
                { FunctionCategory.DominantSub, new() {Chord_CommonNotation.II, Chord_CommonNotation.IV, Chord_CommonNotation.V, Chord_CommonNotation.VII } },
                { FunctionCategory.Subdominant, new() {Chord_CommonNotation.III } },
            }, 
        };
        // function mapping for Ionian with common chords
        private static readonly ScaleChordFunctionBinding _ionianFunctionalBindings = new(){
            CommonChordBindings = new() {
                { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
                { FunctionCategory.TonicSub, new() {Chord_CommonNotation.III, Chord_CommonNotation.VI, } },
                { FunctionCategory.Dominant, new() {Chord_CommonNotation.V } },
                { FunctionCategory.DominantSub, new() {Chord_CommonNotation.V, Chord_CommonNotation.VII, } },
                { FunctionCategory.Subdominant, new() {Chord_CommonNotation.II, Chord_CommonNotation.IV } },
            },
        };
        // function mapping for Mixolydian with common chords
        private static readonly ScaleChordFunctionBinding _mixolydianFunctionalBindings = new(){
            CommonChordBindings = new() {
                { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
                { FunctionCategory.TonicSub, new() {Chord_CommonNotation.I, Chord_CommonNotation.II, Chord_CommonNotation.IV, Chord_CommonNotation.VI } },
                { FunctionCategory.Dominant, new() {Chord_CommonNotation.VII } },
                { FunctionCategory.DominantSub, new() {Chord_CommonNotation.III, Chord_CommonNotation.V, Chord_CommonNotation.VII } },
                { FunctionCategory.Subdominant, new() {Chord_CommonNotation.II } },
            },
        };
        // function mapping for Dorian with common chords
        private static readonly ScaleChordFunctionBinding _dorianFunctionalBindings = new(){
            CommonChordBindings = new() {
                { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
                { FunctionCategory.TonicSub, new() {Chord_CommonNotation.I, Chord_CommonNotation.VI } },
                { FunctionCategory.Dominant, new() {Chord_CommonNotation.IV } },
                { FunctionCategory.DominantSub, new() {Chord_CommonNotation.II, Chord_CommonNotation.IV, Chord_CommonNotation.VI } },
                { FunctionCategory.Subdominant, new() {Chord_CommonNotation.III, Chord_CommonNotation.V, Chord_CommonNotation.VII } },
            },
        };
        // function mapping for Aeolian with common chords
        private static readonly ScaleChordFunctionBinding _aeolianFunctionalBindings = new(){
            CommonChordBindings = new() {
                { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
                { FunctionCategory.TonicSub, new() {Chord_CommonNotation.I, Chord_CommonNotation.III, Chord_CommonNotation.VI } },
                { FunctionCategory.Dominant, new() {Chord_CommonNotation.V } },
                { FunctionCategory.DominantSub, new() {Chord_CommonNotation.V, Chord_CommonNotation.VII } },
                { FunctionCategory.Subdominant, new() {Chord_CommonNotation.II, Chord_CommonNotation.IV } },
            },
        };
        // function mapping for Phrygian with common chords
        private static readonly ScaleChordFunctionBinding _phrygianFunctionalBindings = new(){
            CommonChordBindings = new() {
                { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
                { FunctionCategory.TonicSub, new() {Chord_CommonNotation.I, Chord_CommonNotation.IV, Chord_CommonNotation.VI } },
                { FunctionCategory.Dominant, new() {Chord_CommonNotation.III } },
                { FunctionCategory.DominantSub, new() {Chord_CommonNotation.II, Chord_CommonNotation.III, Chord_CommonNotation.V, Chord_CommonNotation.VII } },
                { FunctionCategory.Subdominant, new() {Chord_CommonNotation.IV } },
            },
        };
        // function mapping for Locrian with common chords
        private static readonly ScaleChordFunctionBinding _locrianFunctionalBindings = new(){
            CommonChordBindings = new() {
                { FunctionCategory.Tonic, new() {Chord_CommonNotation.I } },
                { FunctionCategory.TonicSub, new() {Chord_CommonNotation.I, Chord_CommonNotation.IV } },
                { FunctionCategory.Dominant, new() {Chord_CommonNotation.VI } },
                { FunctionCategory.DominantSub, new() {Chord_CommonNotation.III, Chord_CommonNotation.V, Chord_CommonNotation.VI } },
                { FunctionCategory.Subdominant, new() {Chord_CommonNotation.II, Chord_CommonNotation.VII } },
            },
        };

        #endregion

        #region Data Structure Defenitions

        /// <summary>
        /// Holds bindings for scales' and modes' chord function bindings
        /// </summary>
        public struct ScaleChordFunctionBinding
        {
            // REMEMBER TO ADD CLONING FUNCTIONALITY FOR OTHER BINDINGS WHEN ADDING NON-COMMON CHORDS

            /// <summary>
            /// Bindings for common chords
            /// </summary>
            public Dictionary<FunctionCategory, List<Chord_CommonNotation>> CommonChordBindings;


            /// <summary>
            /// Creates a deep clone of the binding. Writing to sublist within dictionary will not update original lists, new lists created. 
            /// </summary>
            /// <returns></returns>
            public ScaleChordFunctionBinding Clone()
            {
                ScaleChordFunctionBinding clone = new(){
                    CommonChordBindings = new()
                };
                // clone common chord dictionary and sublists
                foreach (FunctionCategory functionCategory in CommonChordBindings.Keys){
                    clone.CommonChordBindings.Add(functionCategory, new());
                    for (int i = 0; i < CommonChordBindings[functionCategory].Count; i++){
                        clone.CommonChordBindings[functionCategory].Add(CommonChordBindings[functionCategory][i]); //copies over
                    }
                }

                return clone;
            }

            // Extendable, we can later add more bindings for other chord notations to get non-diatonic, borrowed, and non-tertian chords. 
            // These need to be worked into above defenitions and grammar algorithms. 
        }

        #endregion

    }

    /// <summary>
    /// Contaims hardcoded bindings for closure sequences within a section and a function to get one back randomly, supports seeds
    /// </summary>
    public static class FunctionalClosureBindings
    {
        /// <summary>
        /// Get a random closure sequence, a series of 2-4 orgin symbols that define the passages within a section
        /// </summary>
        /// <param name="seed"> optional seed. Incremented by functiom </param>
        /// <returns> Closure sequence </returns>
        public static List<PassageClosure> GetRandomClosureSequence(Seed seed)
        {
            System.Random rand = Seed.GetSeededGenerator(seed);

            return _passageClosureSequences[rand.Next(0, _passageClosureSequences.Count)];
        }

        // sequences of passage closures, used as grammatical orgin symbols
        private static readonly List<List<PassageClosure>> _passageClosureSequences = new(){
            new() {PassageClosure.Closed, PassageClosure.Closed }, //cc
            new() {PassageClosure.Closed, PassageClosure.Open }, //co
            new() {PassageClosure.Closed, PassageClosure.Open, PassageClosure.Closed }, //coc
            new() {PassageClosure.Closed, PassageClosure.Open, PassageClosure.Open, PassageClosure.Closed }, //cooc
        };
    }

    #endregion


    #region Utility

    public static class MusicGenUtility
    {
        /// <summary>
        /// Get the diatonic mode mapped to a specified valence value
        /// </summary>
        /// <returns></returns>
        public static DiatonicMode GetModeFromValence(float valence)
        {
            //select mode from valence, uses mapping from enum
            float modalValence = Mathf.Clamp(valence, -1f, 0.99f);
            return (DiatonicMode)(Mathf.FloorToInt((modalValence + 1) * 3f) + 1);
        }

        /// <summary>
        /// Get a diatonic scale via root note, and mode specification
        /// </summary>
        /// <param name="root"> root note for the diatonic scale </param>
        /// <param name="scaleMode"> mode for the scale </param>
        /// <returns></returns>
        public static Scale GetDiatonicScale(NoteName root, DiatonicMode scaleMode = DiatonicMode.Ionian)
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
            for (int i = 0; i < scaleIntervals.Length; i++)
            { //populates interval sequence
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
        public static Chord GetDiatonicChord_Triad(Chord_CommonNotation chord, Scale scale)
        {
            Note[] scaleNotes = scale.GetAscendingNotes(Note.Get(scale.RootNote, 4)).ToArray();
            int startIndex = (int)chord;
            return new(
                scaleNotes[startIndex].NoteName,
                scaleNotes[(startIndex + 2) % scaleNotes.Length].NoteName,
                scaleNotes[(startIndex + 4) % scaleNotes.Length].NoteName
            );
        }

    }

    /// <summary>
    /// Incrementable seed for generator randomness
    /// Features a nullable integer internaly as seed
    /// Has a "BaseSeed" value, and a "ActiveSeed" value. The active seed is set to the base seed when set.
    /// Only the ActiveSeed is modified by incrementation. The BaseSeed remains unchanged
    /// </summary>
    public class Seed
    {
        /// <summary>
        /// Check if the seed exists
        /// </summary>
        public bool HasSeed => _baseSeed.HasValue;
        /// <summary>
        /// read the base seed
        /// </summary>
        public int BaseSeed { get { return _baseSeed ?? int.MinValue; } }
        /// <summary>
        /// Read the seed + its incrementation
        /// </summary>
        public int ActiveSeed { get { return _activeSeed ?? int.MinValue; } }

        private int? _activeSeed;
        private int? _baseSeed;

        /// <summary>
        /// Set the base seed and resets active seed back to the base seed
        /// </summary>
        /// <param name="value"> base value of the new seed </param>
        public void SetSeed(int? value)
        {
            _baseSeed = value.HasValue ? value.Value : null;
            _activeSeed = value.HasValue ? value.Value : null;
        }

        /// <summary>
        /// Change the value of the active seed by a specified number
        /// </summary>
        /// <param name="increment"> number to add to the active seed </param>
        /// <returns> new ActiveSeed Value. if the seed is clear returns the integer minimum </returns>
        public int IncrementSeed(int increment = 1)
        {
            if (_activeSeed != null){
                _activeSeed += increment;
                return _activeSeed.Value;
            }
            else { return int.MinValue; }
        }

        /// <summary>
        /// Remove the seed, setting its values to null
        /// </summary>
        public void ClearSeed()
        {
            _baseSeed = null;
            _activeSeed = null;
        }

        /// <summary>
        /// Gets a system.Random seeded with the ActiveSeed. if seed is clear, a unseeded generator is created. Increments the active seed before using it
        /// </summary>
        /// <param name="seed"> seed to get a generator from </param>
        /// <param name="incrementGenerator"> amount to increment seed by. Set to 0 to not increment at all. 1 is the default. </param>
        /// <returns> Seeded (or unseeded if seed class instance or it's seed are null) system.random number generator </returns>
        public static System.Random GetSeededGenerator(Seed seed, int incrementGenerator = 1) => (seed != null && seed.HasSeed) ? new(seed.IncrementSeed(incrementGenerator)) : new();

    }

    #endregion

}
