using System;
using System.Collections.Generic;
using Sce.Atf.Dom;
using Sce.Atf.Adaptation;

namespace SettingsEditor
{
    public class DynamicSchema
    {
        public DynamicSchema( SettingsCompiler compiler )
        {
            m_compiler = compiler;
        }


        private void SyncProperties( Group group, SettingGroup settingGroup )
        {
            List<DynamicProperty> propertiesCopy = new List<DynamicProperty>();
            foreach ( DynamicProperty dp in group.Properties )
                propertiesCopy.Add( dp );

            group.Properties.Clear();

            // add properties in correct order, creating missing ones along the way
            foreach ( Setting s in settingGroup.Settings )
            {
                DynamicProperty dpOld = propertiesCopy.Find( p => p.Name == s.Name );
                DynamicProperty dp = DynamicProperty.CreateFromSetting( group, s, dpOld );
                group.Properties.Add( dp );
            }
        }

        public void CreateNodesRecurse( DomNode parent, SettingGroup parentStructure )
        {
            // clear children list
            IList<DomNode> childList = null;
            if ( parent.Type == Schema.settingsFileType.Type )
                childList = parent.GetChildList( Schema.settingsFileType.groupChild );
            else
                childList = parent.GetChildList( Schema.groupType.groupChild );

            List<DomNode> childrenCopy = new List<DomNode>();
            foreach ( DomNode dn in childList )
                childrenCopy.Add( dn );

            childList.Clear();

            // add groups in correct order, creating missing groups along the way
            foreach ( SettingGroup structure in parentStructure.NestedStructures)
            {
                DomNode group = childrenCopy.Find( n => n.As<Group>().Name == structure.Name );
                Group g = null;

                if ( group == null )
                {
                    group = new DomNode( Schema.groupType.Type );

                    g = group.As<Group>();
                    g.Name = structure.Name;
                }
                else
                {
                    g = group.As<Group>();
                }

                childList.Add( group );

                g.SettingGroup = structure;

                SyncProperties( g, structure );

                if ( structure.HasPresets )
                {
                    foreach ( Preset p in g.Presets )
                    {
                        Group pGroup = p.As<Group>();
                        SyncProperties( pGroup, structure );
                    }
                }
                else
                {
                    g.Presets.Clear();
                }

                CreateNodesRecurse( group, structure );
            }

        }

        public void CreateNodes( DomNode rootNode )
        {
            CreateNodesRecurse( rootNode, m_compiler.RootStructure );
        }

        private int[] GetEnumIntValues( Type type )
        {
            System.Array valuesArray = Enum.GetValues( type );
            int[] intArray = new int[valuesArray.Length];
            for (int i = 0; i < valuesArray.Length; ++i)
            {
                intArray[i] = (int) valuesArray.GetValue( i );
            }
            return intArray;
        }

        SettingsCompiler m_compiler;
    }
}
