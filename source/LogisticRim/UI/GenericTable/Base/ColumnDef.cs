using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LogisticRim
{
    internal class ColumnDef<T>
    {
        public ColumnWorker<T> Worker
        {
            get
            {
                if ( this.workerInt == null )
                {
                    this.workerInt = (ColumnWorker<T>)Activator.CreateInstance( this.workerClass );
                    this.workerInt.def = this;
                }
                return this.workerInt;
            }
        }

        public Texture2D HeaderIcon
        {
            get
            {
                if ( this.headerIconTex == null && !this.headerIcon.NullOrEmpty() )
                {
                    this.headerIconTex = ContentFinder<Texture2D>.Get( this.headerIcon, true );
                }
                return this.headerIconTex;
            }
        }

        public Vector2 HeaderIconSize
        {
            get
            {
                if ( this.headerIconSize != default( Vector2 ) )
                {
                    return this.headerIconSize;
                }
                if ( this.HeaderIcon != null )
                {
                    return IconSize;
                }
                return Vector2.zero;
            }
        }

        public bool HeaderInteractable
        {
            get
            {
                return this.sortable || !this.headerTip.NullOrEmpty() || this.headerAlwaysInteractable;
            }
        }

        public Type workerClass;

        public bool sortable;

        public bool ignoreWhenCalculatingOptimalTableSize;

        [NoTranslate]
        public string headerIcon;

        public Vector2 headerIconSize;

        [MustTranslate]
        public string headerTip;

        public bool headerAlwaysInteractable;

        public bool paintable;

        public int gap;

        public int widthPriority;

        public int width = -1;

        [Unsaved( false )]
        private ColumnWorker<T> workerInt;

        [Unsaved( false )]
        private Texture2D headerIconTex;

        public string label;

        private const int IconWidth = 26;

        private static readonly Vector2 IconSize = new Vector2( 26f, 26f );
    }
}