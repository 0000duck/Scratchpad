// HiStreamTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "DagNode.h"
#include <algorithm>

//#define DEFINE_ATTRIBUTES_BEGIN(nodeName) \
//nodeName::nodeName##AttrSetType nodeName::__s##nodeName##AttrSet( nullptr ); \
//
//
//#define DEFINE_ATTRIBUTE(nodeName, attrName, shortName ) \
//const AttributeDesc nodeName::Attribute_##attrName::desc( &__s##nodeName##AttrSet, nodeName::Attribute_##attrName::ordinal, #attrName, shortName, nodeName::Attribute_##attrName::type, offsetof( nodeName, attrName ), nodeName::Attribute_##attrName::hasNetwork ); \

//#define DEFINE_ATTRIBUTES_BEGIN(nodeName, typeName) \
//nodeName::nodeName##TypeDesc nodeName::__s##nodeName##TypeDesc(typeName, nodeName::typeID); \
//
//
//#define DEFINE_ATTRIBUTE(nodeName, attrName, shortName ) \
//const AttributeDesc nodeName::Attribute_##attrName::desc( &__s##nodeName##TypeDesc.attrSet_, nodeName::Attribute_##attrName::ordinal, #attrName, shortName, nodeName::Attribute_##attrName::type, offsetof( nodeName, attrName ), nodeName::Attribute_##attrName::hasNetwork ); \







DEFINE_ATTRIBUTES_BEGIN( DagNode, "dagNode" );

DEFINE_ATTRIBUTE( DagNode, visibility, "pvis" );
DEFINE_ATTRIBUTE( DagNode, sortingPriority, "srtp" );


//DagNode::DagNodeTypeDesc DagNode::__sDagNodeTypeDesc;

struct LoadGraphicsDataSetter
{
	LoadGraphicsDataSetter( NodeTypeDesc& ntd )
	{
		ntd.hasLoadGraphicsData_ = true;
	}
};

LoadGraphicsDataSetter DagNodeSetHasLoadGraphicsData( DagNode::getDagNodeTypeDesc() );
