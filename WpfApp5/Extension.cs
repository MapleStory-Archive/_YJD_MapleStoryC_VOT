using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp5
{
    class Ex
    {
        public static Collection<T> FindAll<T>(DependencyObject parent, string childName)
where T : DependencyObject
        {
            Collection<T> result = null;

            Boolean b = true;
            int c = 0;
            while ( b )
            {
                T obj = FindChild<T>(parent, childName, c, -1);
                if ( obj == null ) b = false;
                else
                {
                    if ( result == null ) result = new Collection<T>();
                    result.Add(obj);
                }
                c++;
            }
            return result;
        }
           /**
      * Param c = count of desired object
     * Param indchild= keeps cound (give negative for calling function)
     * */
    public static T FindChild<T>(DependencyObject parent, string childName, int c, int indchild)
where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if ( parent == null ) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            if ( indchild <= -1 ) indchild = 0;


            for ( int i = 0 ; i < childrenCount ; i++ )
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if ( childType == null )
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName, c, indchild);

                    // If the child is found, break so we do not overwrite the found child. 
                    if ( foundChild != null ) break;
                }
                else if ( !string.IsNullOrEmpty(childName) )
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if ( frameworkElement != null && frameworkElement.Tag != null && (( String )frameworkElement.Tag).Equals(childName) )
                    {
                        // if the child's name is of the request name

                        if ( c == indchild )
                        {
                            foundChild = ( T )child;
                            break;
                        }
                        indchild++;
                    }
                }
            }

            return foundChild;
        }
    }
}
