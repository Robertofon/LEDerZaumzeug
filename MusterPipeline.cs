using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Ein Baum mit Generatoren als Blätter ab <see cref="Root"/>
    /// Dies ist die Exekutor-Klasse. Serialisierter Baum geht rein.
    /// Und ein Exekutor läuft drüber und generiert das finale Muster.
    /// </summary>
    public class MusterPipeline
    {
        public MusterPipeline(MusterNode root)
        {
            this.Root = root;
        }

        public MusterNode Root { get; private set; }

        /// <summary>
        /// Führt die Muster-pipeline aus und generiert mit dieser
        /// Frame-Nummer ein Bild unter Aufruf aller Komponenten des Baums.
        /// </summary>
        /// <param name="frame">Framenummer. Sollte aufsteigend sein.</param>
        /// <returns>RGBPixel-Bild</returns>
        public Task<RGBPixel[,]> ExecuteAsync(long frame)
        {
            return this.ExecuteIntAsync(this.Root, frame);
        }
        
        private async Task<RGBPixel[,]> ExecuteIntAsync(MusterNode m, long frame)
        {
            GeneratorNode gn = m as GeneratorNode;
            if( gn != null)
            {
                var muster = await gn.Inst.GenPattern(frame);
                return muster;
            }

            MixerNode mn = m as MixerNode;
            if( mn != null )
            {
                RGBPixel[][,] allesubquellen = await Task.WhenAll(mn.Quelle.Select(q => this.ExecuteIntAsync(q, frame)));
                RGBPixel[,] muster = await mn.Inst.Mix(allesubquellen, frame);
                return muster;
            }

            FilterNode fn = m as FilterNode;
            if( fn != null )
            {
                RGBPixel[,] gefiltert = await this.ExecuteIntAsync(fn.Quelle, frame);
                RGBPixel[,] muster = await fn.Inst.Filter(gefiltert, frame);
                return muster;
            }

            throw new NotImplementedException($"Unbekannter Knotentyp {m.GetType()}");
        }

        /// <summary>
        /// Aufrufen nach Konstruktion, um die Matrixparameter bekannt zu machen
        /// und um alle beteiligten Objkete zu generieren. Dabei kann natürlich
        /// eine Menge schief gehen - Hier werden Exceptions geworfen.
        /// </summary>
        /// <param name="mparams"></param>
        public void Initialisiere(MatrixParams mparams)
        {
            this.Aktiviere(this.Root, mparams);
        }
        
        
        /// <summary>
        /// Aktivator. Einmal den Baum iterieren, um die
        /// Genrator, Mixer und Filter-Objekte zu instantiieren.
        /// </summary>
        private void Aktiviere(MusterNode node, MatrixParams mparams)
        {
            if (node is MixerNode)
            {
                MixerNode mnode = (MixerNode)node;
                // MixerNode evaluieren und somit erzeugen.
                mnode.Inst.Initialize(mparams);
                foreach (var qnode in mnode.Quelle)
                {
                    // Rekursion
                    this.Aktiviere(qnode, mparams);
                }
            }

            if (node is GeneratorNode)
            {
                GeneratorNode gnode = (GeneratorNode)node;
                // Generatorinstanz erzeugen lassen.
                gnode.Inst.Initialize(mparams);
            }

            if (node is FilterNode)
            {
                FilterNode fnode = (FilterNode)node;
                // Filterinstanz erzeugen durch Zugriff
                fnode.Inst.Initialize(mparams);
                // Rekursion
                this.Aktiviere(fnode.Quelle, mparams);
            }
        }

    }

    public abstract class MusterNode
    {
        public string TypeName { get; set; }

        protected T CreateObjectInstance<T>()
        {
            Type t = Type.GetType(this.TypeName);
            var obj = (T)Activator.CreateInstance(t);
            return obj;
        }

    }


    [Serializable]
    public class GeneratorNode : MusterNode
    {
        [NonSerialized]
        private IGenerator inst;

        [DebuggerHidden]
        internal IGenerator Inst
        {
            get
            {
                return this.inst?? (inst = this.CreateObjectInstance<IGenerator>());
            }
        }
    }


    [Serializable]
    public class FilterNode : MusterNode
    {
        [NonSerialized]
        private IFilter inst;

        [DebuggerHidden]
        internal IFilter Inst
        {
            get
            {
                return this.inst ?? (inst = this.CreateObjectInstance<IFilter>());
            }
        }

        public MusterNode Quelle { get; set; } 
    }


    [Serializable]
    public class MixerNode : MusterNode
    {
        //public string TypeName { get; set; }

        [NonSerialized]
        private IMixer inst;

        [DebuggerHidden]
        internal IMixer Inst
        {
            get
            {
                return this.inst ?? (inst = this.CreateObjectInstance<IMixer>());
            }
        }

        public IList<MusterNode> Quelle { get; set; } = new List<MusterNode>();
    }


}