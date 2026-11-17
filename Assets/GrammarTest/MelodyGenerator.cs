
using System;
using System.Collections.Generic;
using System.Linq;

public class MelodyGenerator
{
    // One unit represents one sixteenth note.
    // A 4/4 measure therefore contains 16 units.

    public struct MelodyEvent
    {
        // Scale degree, including octave displacement.
        public int ScaleDegree;

        // Duration measured in sixteenth-note units.
        public int DurationSixteenths;

        // Chromatic alteration applied when converting to MIDI.
        public int ChromaticOffset;

        public MelodyEvent(int degree, int duration, int offset = 0)
        {
            ScaleDegree = degree;
            DurationSixteenths = duration;
            ChromaticOffset = offset;
        }
    }

    // Configuration
    private const int MAX_ATTEMPTS = 15;
    private const int STARTING_PITCH = 7;
    private const int MIN_PITCH = 0;
    private const int MAX_PITCH = 14;
    private const int FAR_JUMP_THRESHOLD = 3;

    /// <summary>
    /// Main entry point.
    /// Generates rhythm first, then pitch, then applies
    /// the melodic rules.
    /// </summary>
    public List<MelodyEvent> GenerateMelody(
        GrammarTest.Seed seed,
        float arousal,
        float valence,
        int targetNotes = 8)
    {
        arousal = Clamp(arousal, 0f, 1f);
        valence = Clamp(valence, -1f, 1f);

        System.Random rand =
            GrammarTest.Seed.GetSeededGenerator(seed);

        // 1. Generate rhythm
        List<int> rhythm = GenerateRhythm(
            rand, targetNotes);

        // 2. Generate binary string
        string binary = GenerateBinaryString(
            rhythm.Count, rand);

        // 3. Generate initial pitch sequence
        List<int> pitches = GenerateBasePitchSequence(
            binary, rand);

        // 4. Apply melodic rules
        List<int> offsets = new List<int>(
            new int[pitches.Count]);

        ApplyRepetitionRule(pitches);
        ApplyLeapRule(pitches);

        ApplyWeightedRules(
            pitches,
            offsets,
            arousal,
            valence,
            rand
        );

        // 5. Combine rhythm and pitch
        List<MelodyEvent> melody = new();

        for (int i = 0; i < pitches.Count; i++)
        {
            melody.Add(new MelodyEvent(
                pitches[i],
                rhythm[i],
                offsets[i]
            ));
        }

        return melody;
    }

    // --------------------------------------------------
    // RHYTHM GENERATION
    // --------------------------------------------------

    private List<int> GenerateRhythm(
        System.Random rand,
        int targetNotes)
    {
        // Four quarter notes in a 4/4 measure.
        // Each quarter note = four sixteenth-note units.

        List<int> rhythm = new()
        {
            4, 4, 4, 4
        };

        int attempts = 0;

        while (rhythm.Count < targetNotes &&
               attempts < MAX_ATTEMPTS)
        {
            // Find the first largest subdivision.
            int largestIndex = -1;
            int largestDuration = 0;

            for (int i = 0; i < rhythm.Count; i++)
            {
                if (rhythm[i] > largestDuration &&
                    rhythm[i] >= 2)
                {
                    largestDuration = rhythm[i];
                    largestIndex = i;
                }
            }

            if (largestIndex == -1)
                break;

            // Divide the selected duration in half.
            int firstHalf = largestDuration / 2;
            int secondHalf = largestDuration - firstHalf;

            rhythm[largestIndex] = firstHalf;

            rhythm.Insert(
                largestIndex + 1,
                secondHalf
            );

            attempts++;
        }

        return rhythm;
    }

    // --------------------------------------------------
    // BINARY STRING GENERATION
    // --------------------------------------------------

    private string GenerateBinaryString(
        int length,
        System.Random rand)
    {
        char[] bits = new char[length];

        for (int i = 0; i < length; i++)
        {
            bits[i] = rand.Next(0, 2) == 0
                ? '0'
                : '1';
        }

        return new string(bits);
    }

    // --------------------------------------------------
    // BASE PITCH GENERATION
    // --------------------------------------------------

    private List<int> GenerateBasePitchSequence(
        string binary,
        System.Random rand)
    {
        List<int> pitches = new();

        int currentPitch = STARTING_PITCH;

        for (int i = 0; i < binary.Length; i++)
        {
            if (i > 0)
            {
                // Binary value determines direction.
                int direction = binary[i] == '1'
                    ? 1
                    : -1;

                currentPitch += direction;
            }

            currentPitch = Clamp(
                currentPitch,
                MIN_PITCH,
                MAX_PITCH
            );

            pitches.Add(currentPitch);
        }

        return pitches;
    }

    // --------------------------------------------------
    // MELODIC RULE 1:
    // REMOVE THIRD OR MORE CONSECUTIVE REPETITIONS
    // --------------------------------------------------

    private void ApplyRepetitionRule(
        List<int> pitches)
    {
        for (int i = 2; i < pitches.Count; i++)
        {
            if (pitches[i] == pitches[i - 1] &&
                pitches[i] == pitches[i - 2])
            {
                int direction =
                    pitches[i] < STARTING_PITCH ? 1 : -1;

                pitches[i] = Clamp(
                    pitches[i] + direction,
                    MIN_PITCH,
                    MAX_PITCH
                );
            }
        }
    }

    // --------------------------------------------------
    // MELODIC RULE 2:
    // STEPWISE MOVEMENT AFTER A LARGE LEAP
    // --------------------------------------------------

    private void ApplyLeapRule(
        List<int> pitches)
    {
        for (int i = 1; i < pitches.Count - 1; i++)
        {
            int interval =
                pitches[i] - pitches[i - 1];

            if (Math.Abs(interval) >= FAR_JUMP_THRESHOLD)
            {
                // Move one scale degree in the opposite
                // direction of the leap.

                int recoveryDirection =
                    interval > 0 ? -1 : 1;

                pitches[i + 1] = Clamp(
                    pitches[i] + recoveryDirection,
                    MIN_PITCH,
                    MAX_PITCH
                );
            }
        }
    }

    // --------------------------------------------------
    // MELODIC RULES 3, 4, AND 5:
    // WEIGHTED ADJUSTMENTS
    // --------------------------------------------------

    private void ApplyWeightedRules(
        List<int> pitches,
        List<int> offsets,
        float arousal,
        float valence,
        System.Random rand)
    {
        // Normalize valence because it ranges from -1 to 1.
        float normalizedValence = (valence + 1f) / 2f;

        float closeJumpWeight =
            2f * (1f - arousal) + 1f;

        float farJumpWeight =
            2f * arousal;

        float dissonanceWeight =
            3f * normalizedValence;

        float rangeWeight =
            2f * arousal + 1f;

        for (int i = 1; i < pitches.Count; i++)
        {
            float totalWeight =
                closeJumpWeight +
                farJumpWeight +
                dissonanceWeight +
                rangeWeight;

            float selection =
                (float)rand.NextDouble() * totalWeight;

            int direction =
                pitches[i] >= pitches[i - 1] ? 1 : -1;

            // CLOSE JUMP
            if (selection < closeJumpWeight)
            {
                // Keep a small interval.
                pitches[i] = pitches[i - 1] + direction;
            }

            // FAR JUMP
            else if (selection <
                closeJumpWeight + farJumpWeight)
            {
                // Larger melodic movement.
                pitches[i] = pitches[i - 1] +
                    (direction * FAR_JUMP_THRESHOLD);
            }

            // DISSONANCE
            else if (selection <
                closeJumpWeight + farJumpWeight +
                dissonanceWeight)
            {
                // Chromatic alteration is applied later
                // when converting the scale degree to MIDI.
                offsets[i] = rand.Next(0, 2) == 0 ? -1 : 1;
            }

            // RANGE BIAS
            else
            {
                // Encourage pitches to return toward
                // the center of the melodic range.
                int center = STARTING_PITCH;

                if (pitches[i] > center)
                    pitches[i]--;

                else if (pitches[i] < center)
                    pitches[i]++;
            }

            pitches[i] = Clamp(
                pitches[i],
                MIN_PITCH,
                MAX_PITCH
            );
        }
    }

    // --------------------------------------------------
    // UTILITY
    // --------------------------------------------------

    private int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    private float Clamp(float value, float min, float max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
}
