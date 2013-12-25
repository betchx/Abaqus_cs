using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;



namespace AbaqusConvergenceMonitor
{
    static class MatchAdupter
    {
        public static int to_i(this Match m, int pos = 1)
        {
            int res = 0;
            return int.TryParse(m.Groups[pos].Captures[0].Value, out res) ? res : 0;
        }
        public static double to_f(this Match m, int pos = 1)
        {
            double res = 0;
            return double.TryParse(m.Groups[pos].Captures[0].Value, out res) ? res : 0.0;
        }
    }

    class IterationInfo
    {
        int step;
        int increment;
        int attempt;
        int iteration;
        double average_force;
        double residual_force;
        bool force_converged;
        double average_monent;
        double residual_moment;
        bool moment_converged;
        double time_increment;

        enum State
        {
            Step,  Attempt, Iteration,
            AverageForce,  ResidualForce,  DispIncrement, DispCorrection, ForceConverged,
            AverageMoment, ResidualMoment, RotIncrement,  RotCorrection,  MomentConverged,
            Finished,  Next,
        };

        delegate State MsgParser(string line);
        Dictionary<State, MsgParser> pMap;
        State current;


        readonly static Regex re_step;// = new Regex(@"^ +S T E P +(\d+) +");
        readonly static Regex re_attempt;// = new Regex(@"^ +INCREMENT +(\d+) STARTS. ATTEMPT NUMBER +(\d+), TIME INCREMENT  (\W+) +");
        readonly static Regex re_iteration;// = new Regex(@" CONVERGENCE CHECKS FOR SEVERE DISCONTINUITY ITERATION +(\W+) +");
        readonly static Regex re_average_force;// = new Regex(@" AVERAGE FORCE +(\W+) +");
        readonly static Regex re_residual_force;// = new Regex(@"LARGEST RESIDUAL FORCE +(\W+)  +AT");
        readonly static Regex re_force_converged;// = new Regex(@" THE FORCE     EQUILIBRIUM EQUATIONS HAVE CONVERGED");
//        readonly static Regex re_force_not_converged = new Regex(@"FORCE     EQUILIBRIUM NOT ACHIEVED WITHIN TOLERANCE");
        readonly static Regex re_average_moment;// = new Regex(@"AVERAGE MOMENT +(\W+) +TIME");
        readonly static Regex re_residual_moment;// = new Regex(@"LARGEST RESIDUAL MOMENT +(\W+) +AT");
        readonly static Regex re_moment_ocnverged;// = new Regex(@"THE MOMENT    EQUILIBRIUM EQUATIONS HAVE CONVERGED");
        readonly static Regex re_next;

        static IterationInfo()
        {
            re_step = new Regex(@"^ +S T E P +(\d+) +");
            re_attempt = new Regex(@" +INCREMENT +(\d+) STARTS. ATTEMPT NUMBER +(\d+), TIME INCREMENT +(\S+)");
            re_iteration = new Regex(@" CONVERGENCE CHECKS FOR SEVERE DISCONTINUITY ITERATION +(\S+)");
            re_average_force = new Regex(@" AVERAGE FORCE +(\S+) +");
            re_residual_force = new Regex(@"LARGEST RESIDUAL FORCE +(\S+)  +AT");
            re_force_converged = new Regex(@" THE FORCE     EQUILIBRIUM EQUATIONS HAVE CONVERGED");
            //re_force_not_converged = new Regex(@"FORCE     EQUILIBRIUM NOT ACHIEVED WITHIN TOLERANCE");
            re_average_moment = new Regex(@"AVERAGE MOMENT +(\S+) +TIME");
            re_residual_moment = new Regex(@"LARGEST RESIDUAL MOMENT +(\S+) +AT");
            re_moment_ocnverged = new Regex(@"THE MOMENT    EQUILIBRIUM EQUATIONS HAVE CONVERGED");
            re_next = new Regex(@"FRACTION OF STEP COMPLETED +(\S+)");

        }

        private int to_i(Match m, int pos = 0)
        {
            return int.Parse(m.Groups[pos].Captures[0].Value);
        }
        private double to_f(Match m, int pos = 0)
        {
            return double.Parse(m.Groups[pos].Captures[0].Value);
        }

        State ParseStep(string line)
        {
            var ans = re_step.Match(line);
            if (ans.Success)
            {
                step = ans.to_i();
                return State.Attempt;
            }
            return State.Step;
        }

        State ParseAttempt(string line)
        {
            var res = re_attempt.Match(line);
            if (res.Success)
            {
                increment = res.to_i(1);
                attempt =res.to_i(2);
                time_increment = res.to_f(3);
                return State.Iteration;
            }
            return State.Attempt;
        }

        State ParseIteration(string line)
        {
            var ans = re_iteration.Match(line);
            if (ans.Success)
            {
                iteration = ans.to_i();
                return State.AverageForce;
            }
            return State.Iteration;
        }

        State ParseAverageForce(string line)
        {
            var ans = re_average_force.Match(line);
            if (ans.Success)
            {
                average_force = ans.to_f();
                return State.ResidualForce;
            }
            return State.AverageForce;
        }

        State ParseResidualForce(string line)
        {
            residual_force = re_residual_force.Match(line).to_f();
            return State.DispIncrement;
        }

        State ParseForceConvergence(string line)
        {
            force_converged = re_force_converged.IsMatch(line);
            return State.AverageMoment;
        }

        State ParseAverageMoment(string line)
        {
            var ans = re_average_moment.Match(line);
            if (ans.Success)
            {
                average_monent = ans.to_f();
                return State.ResidualMoment;
            }
            return State.AverageMoment;
        }

        State ParseResidualMoment(string line)
        {
            residual_moment = re_residual_moment.Match(line).to_f();
            return State.RotIncrement;
        }

        State ParseMomentConvergence(string line)
        {
            moment_converged = re_moment_ocnverged.IsMatch(line);
            return State.Finished;
        }

        State ParseNext(string line)
        {
            var ans = re_next.Match(line);
            if (ans.Success)
            {
                if (ans.Groups[1].Value == "1.00")
                {
                    return State.Step;
                }
                return State.Attempt;
            }
            return State.Next;
        }

        public IterationInfo()
        {
            pMap = new Dictionary<State, MsgParser>();
            pMap.Add(State.Step, ParseStep);
            pMap.Add(State.Attempt, ParseAttempt);
            pMap.Add(State.Iteration, ParseIteration);
            pMap.Add(State.AverageForce, ParseAverageForce);
            pMap.Add(State.ResidualForce, ParseResidualForce);
            pMap.Add(State.DispIncrement, delegate(string s) { return State.DispCorrection; });
            pMap.Add(State.DispCorrection, delegate(string s) { return State.ForceConverged; });
            pMap.Add(State.ForceConverged, ParseForceConvergence);
            pMap.Add(State.AverageMoment, ParseAverageMoment);
            pMap.Add(State.ResidualMoment, ParseResidualMoment);
            pMap.Add(State.RotIncrement, delegate(string s) { return State.RotCorrection; });
            pMap.Add(State.RotCorrection, delegate(string s) { return State.MomentConverged; });
            pMap.Add(State.MomentConverged, ParseMomentConvergence);
            pMap.Add(State.Finished, delegate(string s) { if (force_converged && moment_converged) return State.Next; return State.Iteration; });
            pMap.Add(State.Next, ParseNext);

            current = State.Step;
        }

        public bool Add(string line)
        {
            current = pMap[current](line);
            return current == State.Finished;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("I: {0,4},{1,5},{2,2}, {3,10}, {4,2}",
                step, increment, attempt, time_increment, iteration);
            sb.AppendFormat(" F: {0,10},{1,10},{2,5}", average_force, residual_force, force_converged);
            sb.AppendFormat(" M: {0,10},{1,10},{2,5}", average_monent, residual_moment, moment_converged);
            return sb.ToString();
        }

        public void Reset()
        {
            current = State.Step;
        }
    }
}
