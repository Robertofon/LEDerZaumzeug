﻿using System;
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
    public class MusterPipeline : IDisposable
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
        public Task<RGBPixel[,]> ExecuteAsync(ulong frame)
        {
            return this.ExecuteIntAsync(this.Root, frame);
        }
        
        private async Task<RGBPixel[,]> ExecuteIntAsync(MusterNode m, ulong frame)
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
                try
                {
                    mnode.Inst.Initialize(mparams);
                }
                catch (System.Exception ex)
                {
                    throw new MusterPipelineException(mnode, ex);
                }
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
                try
                {
                    gnode.Inst.Initialize(mparams);
                }
                catch (System.Exception ex)
                {
                    throw new MusterPipelineException(gnode, ex);
                }
            }

            if (node is FilterNode)
            {
                FilterNode fnode = (FilterNode)node;
                // Filterinstanz erzeugen durch Zugriff
                try
                {
                    fnode.Inst.Initialize(mparams);                    
                }
                catch (System.Exception ex)
                {
                    throw new MusterPipelineException(fnode, ex);
                }
                // Rekursion
                this.Aktiviere(fnode.Quelle, mparams);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // verwalteten Zustand (verwaltete Objekte) entsorgen.
                    RekursivesDispose(this.Root);
                }

                // TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
                // TODO: große Felder auf Null setzen.

                disposedValue = true;
            }
        }

        private static void RekursivesDispose(MusterNode node)
        {
            if(node is GeneratorNode)
            {
                ((GeneratorNode)node).Inst.Dispose();
            }
            else if(node is FilterNode)
            {
                ((FilterNode)node).Inst.Dispose();
                RekursivesDispose(((FilterNode)node).Quelle);
            }
            else if(node is MixerNode)
            {
                ((MixerNode)node).Inst.Dispose();
                foreach (MusterNode m in ((MixerNode)node).Quelle)
                {
                    RekursivesDispose(m);
                }
            }

        }

        ~MusterPipeline()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(false);
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            // Für, wenn der Finalizer weiter oben überschrieben wird.
            GC.SuppressFinalize(this);
        }
        #endregion

    }

    public class MusterPipelineException : System.Exception
    {
        public MusterPipelineException(MusterNode mn, System.Exception inner)
            :base ("Node : " + mn.ToString() + " - " + inner, inner)
        {
            this.Node = mn;
        }

        public MusterNode Node { get; private set; }

        // public override string ToString()
        // {
        //     return "Node: " + Node.ToString() + 
        // } 
    }

}